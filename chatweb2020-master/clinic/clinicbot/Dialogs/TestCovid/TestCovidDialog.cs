using clinicbot.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace clinicbot.Dialogs.TestCovid
{
    public class TestCovidDialog : ComponentDialog
    {
        public static string enfermedadRespiratoria;
        public static string Sintomas;
        public static string Intensidad;
        public static string Contacto;
        public TestCovidDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialQuestion,
                AskRespiratory,
                AskSymptoms,
                IntensitySymptoms,
                ContactWithSick,
                FinalDialog
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        private async Task<DialogTurnResult> InitialQuestion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string text = "Podemos realizar un test virtual para saber si tienes COVID-19 ¿Quieres intentarlo?";
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = Confirmation(text)
                },
                cancellationToken
             );
        }

        private async Task<DialogTurnResult> AskRespiratory(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userOption = stepContext.Context.Activity.Text.Trim().ToLower();

            if (userOption.Equals("si"))
            {
                string text = "Listo, aquí vamos. ¿Sufres alguna atención o enfermedad respiratoria?";

                return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = Confirmation(text)
                },
                cancellationToken
             );
            }
            else if (userOption.Equals("no"))
            {
                await stepContext.Context.SendActivityAsync("No hay problema, de todas formas contamos con información sobre la enfermedad. Pudes hacer cualquier consulta.", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog), cancellationToken: cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AskSymptoms(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userOption = stepContext.Context.Activity.Text.Trim().ToLower();
            if (userOption.Equals("si") || userOption.Equals("no"))
            {
                enfermedadRespiratoria = stepContext.Context.Activity.Text.Trim();
                string messageSymptoms = $"¿Actualmente presenta algunos de estos síntomas?" +
                    $"{Environment.NewLine}Fiebre o escalofríos" +
                    $"{Environment.NewLine}Tos" +
                    $"{Environment.NewLine}Dificultad para respirar" +
                    $"{Environment.NewLine}Fatiga" +
                    $"{Environment.NewLine}Dolores musculares y corporales" +
                    $"{Environment.NewLine}Dolores de cabeza" +
                    $"{Environment.NewLine}Pérdida reciente del olfato o el gusto" +
                    $"{Environment.NewLine}Dolor de garganta" +
                    $"{Environment.NewLine}Nauseas o vómitos" +
                    $"{Environment.NewLine}Diarrea";

                return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = ConfirmationSymptoms(messageSymptoms)
                },
                cancellationToken
             );
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog), cancellationToken: cancellationToken);
            }
        }

        private async Task<DialogTurnResult> IntensitySymptoms(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userOption = stepContext.Context.Activity.Text.Trim().ToLower();

            if (userOption.Equals("solo uno") || userOption.Equals("más de uno") || userOption.Equals("mas de uno"))
            {
                Sintomas = stepContext.Context.Activity.Text.Trim();
                string text = "¿Con qué intensidad presentas estos síntomas?";

                return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = ConfirmationLevel(text)
                },
                cancellationToken
             );
            }
            else if (userOption.Equals("ninguno"))
            {
                Sintomas = stepContext.Context.Activity.Text.Trim();
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog), cancellationToken: cancellationToken);
            }
        }
        private async Task<DialogTurnResult> ContactWithSick(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userOption = stepContext.Context.Activity.Text.Trim().ToLower();

            if (userOption.Equals("suave") || userOption.Equals("fuerte") || userOption.Equals("ninguno"))
            {
                Intensidad = stepContext.Context.Activity.Text.Trim();
                string text = "¿Recientemente ha estado en contacto con alguna persona infectada o en contacto con mucha gente?";

                return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                    {
                        Prompt = Confirmation(text)
                    },
                    cancellationToken
                );
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog), cancellationToken: cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userOption = stepContext.Context.Activity.Text.Trim().ToLower();

            if (userOption.Equals("si"))
            {
                Contacto = stepContext.Context.Activity.Text.Trim();
                string messageFinal = $"DIAGNÓSTICO:" +
                    $"{Environment.NewLine}Enfermedad respíratoria: {enfermedadRespiratoria}" +
                    $"{Environment.NewLine}Síntomas: {Sintomas}" +
                    $"{Environment.NewLine}Intensidad: {Intensidad}" +
                    $"{Environment.NewLine}Contacto: {Contacto}" +
                    $"{Environment.NewLine}Es probable que tengas COVID-19, por favor acude a un centro médico para que te hagan la prueba molecular y recibir la información de profesionales. Respeta el aislamiento, saldrás de esta...";
                await stepContext.Context.SendActivityAsync(messageFinal, cancellationToken: cancellationToken);
            }
            else if (userOption.Equals("no"))
            {
                Contacto = stepContext.Context.Activity.Text.Trim();
                string messageFinal = $"DIAGNÓSTICO:" +
                    $"{Environment.NewLine}Enfermedad respíratoria: {enfermedadRespiratoria}" +
                    $"{Environment.NewLine}Síntomas: {Sintomas}" +
                    $"{Environment.NewLine}Intensidad: {Intensidad}" +
                    $"{Environment.NewLine}Contacto: {Contacto}" +
                    $"{Environment.NewLine}Es probable que tengas COVID-19, por favor acude a un centro médico para que te hagan la prueba molecular y recibir la información de profesionales. Sigue respetando el aislamiento.";
                await stepContext.Context.SendActivityAsync(messageFinal, cancellationToken: cancellationToken);
                await Task.Delay(1500);
                string LastMessage = "Lo más probable es que no tengas COVID-19 por no presentar síntomas claros. No te preocupes que todo saldrá bien. Suerte.";
                await stepContext.Context.SendActivityAsync(LastMessage, cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(RootDialog), cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        #region Methods
        private Activity Confirmation(string text)
        {
            var reply = MessageFactory.Text(text);
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title="Si",Value="Si", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="No",Value="No", Type=ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }
        private Activity ConfirmationSymptoms(string text)
        {
            var reply = MessageFactory.Text(text);
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title="Solo uno",Value="Solo uno", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="Más de uno",Value="Más de uno", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="Ninguno",Value="Ninguno", Type=ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }
        private Activity ConfirmationLevel(string text)
        {
            var reply = MessageFactory.Text(text);
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title="Suave",Value="Suave", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="Fuerte",Value="Fuerte", Type=ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }
        #endregion
    }
}
