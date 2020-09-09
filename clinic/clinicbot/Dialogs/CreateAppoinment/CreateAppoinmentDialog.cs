using clinicbot.Common.Models.BotState;
using clinicbot.Common.Models.EntityLuis;
using clinicbot.Common.Models.MedicalAppointment;
using clinicbot.Common.Models.User;
using clinicbot.Data;
using clinicbot.infrastructura.Luis;
using clinicbot.infrastructura.SendGridEmail;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace clinicbot.Dialogs.CreateAppoinment
{
    public class CreateAppoinmentDialog:ComponentDialog
    {
        public readonly IDataBaseService _dataBaseService;
        public static UserModel newUserModel = new UserModel();
        public static MedicalAppointmentModel medicalAppointmentModel = new MedicalAppointmentModel();
        private readonly ISendGridEmailService _sendGridEmailService;

        private readonly IStatePropertyAccessor<BotStateModel> _userState;
        static string userText;
        private readonly ILuisService _luisService;

        public CreateAppoinmentDialog(IDataBaseService dataBaseService , UserState userState , ISendGridEmailService sendGridEmailService , ILuisService luisService)
        {
            _luisService = luisService;
            _sendGridEmailService = sendGridEmailService;
            _userState = userState.CreateProperty<BotStateModel>(nameof(BotStateModel));
            _dataBaseService = dataBaseService;
            var waterfallStep = new WaterfallStep[]
                {
                    SetPhone,
                    SetFullName,
                    SetEmail,
                    SetDate,
                    SetTime,
                    Confirmation,
                    FinalProcess
       
                };
            
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

        }


     

        private async Task<DialogTurnResult> SetPhone(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            userText = stepContext.Context.Activity.Text;
            var userStateModel = await _userState.GetAsync(stepContext.Context,()=> new BotStateModel()) ;
            if(userStateModel.medicalData)
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(
               nameof(TextPrompt),
               new PromptOptions { Prompt = MessageFactory.Text("Por Favor ingresa tu numero de telefono:") },
               cancellationToken
               );
            }


        }
        private async Task<DialogTurnResult> SetFullName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateModel = await _userState.GetAsync(stepContext.Context, () => new BotStateModel());
            if (userStateModel.medicalData)
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }
            else
            {
                var userPhone = stepContext.Context.Activity.Text;
                newUserModel.phone = userPhone;

                return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text("Ahora ingresa tu nombre completo:") },
                    cancellationToken
                    );
            }
            
        }

        private async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateModel = await _userState.GetAsync(stepContext.Context, () => new BotStateModel());
            if (userStateModel.medicalData)
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }

            else
            {
                var fullNameUser = stepContext.Context.Activity.Text;
                newUserModel.fullName = fullNameUser;

                return await stepContext.PromptAsync(
                     nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text($"Genial ,  Ahora indicame tu Correo") },
                    //    Prompt = MessageFactory.Text($"Genial{name} ,  Ahora ingresa tu Correo"),

                    cancellationToken
                    );
            }
          
        }

        private async Task<DialogTurnResult> SetDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var fullNameUser = stepContext.Context.Activity.Text;
            newUserModel.fullName = fullNameUser;


            var userEmail = stepContext.Context.Activity.Text;
            newUserModel.email = userEmail;
            var newStepContext = stepContext;
            newStepContext.Context.Activity.Text = userText;
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(newStepContext.Context , cancellationToken);
            var Entity = luisResult.Entities.ToObject<EntityLuisModel>();
            if(Entity.datetime != null)
            {
                var date = Entity.datetime.First().timex.First().Replace("XXXX", DateTime.Now.Year.ToString());
                if (date.Length>10)
                    date = date.Remove(10);
                medicalAppointmentModel.date = DateTime.Parse(date);
                return await stepContext.NextAsync(cancellationToken:cancellationToken);
            }
            else
            {
                string text = $" Genial  Ahora necesito la fecha de la cita medica con el siguiente formato" +
               $"{Environment.NewLine}dd//mm/yyyy";

                return await stepContext.PromptAsync(
                   nameof(TextPrompt),
                  new PromptOptions { Prompt = MessageFactory.Text(text) },
                  cancellationToken
                  );
            }
           
        }
    

        private async Task<DialogTurnResult> SetTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if(medicalAppointmentModel.date == DateTime.MinValue)
            {
                var medicalDate = stepContext.Context.Activity.Text;
                medicalAppointmentModel.date = Convert.ToDateTime(medicalDate);
            }

           

            return await stepContext.PromptAsync(
              nameof(TextPrompt),
             new PromptOptions { Prompt = CreateButtonsTime() },
             cancellationToken
             );
        }

       
        private async Task<DialogTurnResult> Confirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var medicalTime = stepContext.Context.Activity.Text;
            medicalAppointmentModel.time = int.Parse(medicalTime);
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions{Prompt=CreateButtonConfirmation() },
                cancellationToken
                );
        }

       

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userConfirmation = stepContext.Context.Activity.Text;
            if(userConfirmation.ToLower().Equals("si"))
            {
                //GUARDAR BASE DE DATOS
                string userId = stepContext.Context.Activity.From.Id;
                var userModel = await _dataBaseService.User.FirstOrDefaultAsync(x => x.id == userId);

                var userStateModel = await _userState.GetAsync(stepContext.Context,() => new BotStateModel());

                if(!userStateModel.medicalData)
                {
                    //UDATE USER
                    userModel.phone = newUserModel.phone;
                    userModel.fullName = newUserModel.fullName;
                    userModel.email = newUserModel.email;

                    _dataBaseService.User.Update(userModel);
                    await _dataBaseService.SaveAsync();
                }

                 


                // GUARDAR LA CITA MEDICA
                medicalAppointmentModel.id = Guid.NewGuid().ToString();
                medicalAppointmentModel.idUser = userId;
                await _dataBaseService.MedicalAppointment.AddAsync(medicalAppointmentModel);
                await _dataBaseService.SaveAsync();

                await stepContext.Context.SendActivityAsync("TU CITA SE GUARDO CON EXITO." , cancellationToken:cancellationToken);
                userStateModel.medicalData = true;
                //MOSTRAR LA INFORMACION
                string summaryMedical = $"Para :{userModel.fullName}" +
                    $"{Environment.NewLine}📞 Telefono: {userModel.phone}"+
                       $"{Environment.NewLine}📧 Email:{userModel.email}"+
                         $"{Environment.NewLine}📅 Fecha:{ medicalAppointmentModel.date}"+
                     $"{Environment.NewLine}⏰ Hora:{ medicalAppointmentModel.time}";
                await stepContext.Context.SendActivityAsync(summaryMedical, cancellationToken:cancellationToken);
                //SEND EMAIL
                await SendEmail(userModel,medicalAppointmentModel);


                await Task.Delay(1000);
                await stepContext.Context.SendActivityAsync("¿En qué  más puedo ayudarte ?",cancellationToken:cancellationToken);

                medicalAppointmentModel = new MedicalAppointmentModel();




            }
            else
            {
                await stepContext.Context.SendActivityAsync("No hay problema, sera la proxima",cancellationToken :cancellationToken);

            }
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task SendEmail(UserModel userModel, MedicalAppointmentModel medicalAppointmentModel)
        {
            string contentEmail = $"Hola  {userModel.fullName} , </b> <b>Tu cita medica :" +
             $"<br>📅Fecha: {medicalAppointmentModel.date.ToShortDateString()}" +
             $"<br>⏰ Hora: {medicalAppointmentModel.time}</b><b> horas."+
               $"<br> "+
                  $"<br> Eecuerda presentarte 15 minutos antes de tu cita y entregar tu carnet al asistente mèdico." +
                $"<br> " +
            $"<br> Siempre hay una solución para cada problema, una sonrisa para cada lágrima y un abrazo (aunque sea virtual) para cada tristeza."+
            $"<br> "+
             $"  <br> En  VIDACOVID   estamos atentos y comprometidos para vivir al máximo el elemento más importante de nuestra cultura: “Las personas van primero”.Cuenten con nosotros para enfrentar este difícil momento. " +
             $" <br>  " +
              $"<br> Ayudame entrando al link para sacarte un analisis " +
           
                        $"<br> " +
                      $"<br> " +
                $"<br>Cordialmente" +
                 $"<br> " +
                  $"<br> DANIEL TATAJE ALARCON " +
                   $"<br> CEO";

            await _sendGridEmailService.Execute(
                "daniel_eldiez@hotmail.com",
           "BOT CITA MEDICA - VIDACOVID",
                userModel.email,
                userModel.fullName,
                "Confirmacion de Cita" , 
                "",
                contentEmail
                );

        }

        private Activity CreateButtonConfirmation()
        {
            var reply = MessageFactory.Text("Confirmas la creacion de esta cita medica ?");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions=new List<CardAction>()
                {
                    new CardAction(){Title="SI", Value="SI" ,Type = ActionTypes.ImBack},
                        new CardAction(){Title="NO", Value="NO" ,Type = ActionTypes.ImBack},
                }
            };
            return reply as Activity;
        }
        private Activity CreateButtonsTime()
        {
            var reply = MessageFactory.Text("Ahora selecciona la hora");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title="9",Value="9", Type = ActionTypes.ImBack},
                        new CardAction(){Title="10",Value="10", Type = ActionTypes.ImBack},
                            new CardAction(){Title="11",Value="11", Type = ActionTypes.ImBack},
                             new CardAction(){Title="15",Value="15", Type = ActionTypes.ImBack},
                             new CardAction(){Title="16",Value="16", Type = ActionTypes.ImBack},
                              new CardAction(){Title="17",Value="17", Type = ActionTypes.ImBack},
                               new CardAction(){Title="18",Value="18", Type = ActionTypes.ImBack},
                }
            };
            return reply as Activity;
         }
    }
}
