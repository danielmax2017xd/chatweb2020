using clinicbot.Common.Cards;
using clinicbot.Data;
using clinicbot.Dialogs.CreateAppoinment;
using clinicbot.Dialogs.Qualificacion;
using clinicbot.infrastructura.Luis;
using clinicbot.infrastructura.QnAMakerAI;
using clinicbot.infrastructura.SendGridEmail;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace clinicbot.Dialogs
{
    public class RootDialog: ComponentDialog
    {
        private readonly ILuisService _luisService;
        private readonly IDataBaseService _dataBaseService;
        private readonly ISendGridEmailService _sendGridEmailService;
        private readonly IQnAMakerAIService  _qnAMakerAIServive;

        public RootDialog()
        {
            var waterfallStep = new WaterfallStep[]
                {
                    SetName,
                    SetAge,
                    ShowData

                };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog),waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>),ValidateAge));
        }

        private async Task<bool> ValidateAge(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                promptContext.Recognized.Succeeded &&
                promptContext.Recognized.Value>0 &&
                promptContext.Recognized.Value < 9999999
                );
        }

        private async Task<DialogTurnResult> ShowData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
         //   await stepContext.Context.SendActivityAsync("Genial  , dime en que te puedo ayudar" ,cancellationToken:cancellationToken);
            var name = stepContext.Context.Activity.Text;
            return await stepContext.PromptAsync(
                nameof(NumberPrompt<int>),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Genial{name} ,  dime en que te puedo ayudar"),
                
                },
                cancellationToken
                    );
        }

        private async Task<DialogTurnResult> SetAge(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var name = stepContext.Context.Activity.Text;
            return await stepContext.PromptAsync(
                nameof(NumberPrompt<int>),
                new PromptOptions {
                    Prompt = MessageFactory.Text($"Bien{name} , ahora necesito tu DNI"),
                    RetryPrompt = MessageFactory.Text($"Bien{name} , Por favor ingresa correctamente")
                },
                cancellationToken
                    );
               
        }

        private async Task<DialogTurnResult> SetName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Para iniciar una conversacion necesito algunos datos", cancellationToken: cancellationToken);
            await Task.Delay(1000);
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Por favor ingresa tu nombre") },
                cancellationToken
                );
        }

        public RootDialog(ILuisService luisService , IDataBaseService dataBaseService, UserState userState, ISendGridEmailService sendGridEmailService, IQnAMakerAIService qnAMakerAIServive)
        {
            _qnAMakerAIServive = qnAMakerAIServive;
            _sendGridEmailService = sendGridEmailService;
            _dataBaseService = dataBaseService;
            _luisService = luisService;
            var waterfallSteps = new WaterfallStep[]
            {
                InitialProcess,
                FinalProcess
            };
            AddDialog(new QualificationDialog(_dataBaseService));
            AddDialog(new CreateAppoinmentDialog(_dataBaseService, userState, _sendGridEmailService , _luisService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch(topIntent.intent)
            {
                case "Saludar":
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Despedir":
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case "VerOpciones":
                    await IntentVerOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case "VerCentroContacto":
                    await IntentVerCentroContacto(stepContext, luisResult, cancellationToken);
                    break;
                case "Calificar":
                   return await IntentCalificar(stepContext, luisResult, cancellationToken);
                case "CrearCita":
                    return await IntentCrearCita(stepContext, luisResult, cancellationToken);

                case "VerCita":
                     await IntentVerCita(stepContext, luisResult, cancellationToken);
                    break;
                case "None":
                   
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                case "Puno":
                    await IntentPuno(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdos":
                    await IntentPdos(stepContext, luisResult, cancellationToken);

                    break;
                case "Ptres":
                    await IntentPtres(stepContext, luisResult, cancellationToken);

                    break;
                case "Pcuatro":
                    await IntentPcuatro(stepContext, luisResult, cancellationToken);

                    break;
                case "Pcinco":
                    await IntentPcinco(stepContext, luisResult, cancellationToken);

                    break;
                case "Pseis":
                    await IntentPseis(stepContext, luisResult, cancellationToken);

                    break;
                case "Psiete":
                    await IntentPsiete(stepContext, luisResult, cancellationToken);

                    break;
                case "Pocho":
                    await IntentPocho(stepContext, luisResult, cancellationToken);

                    break;
                case "Pnueve":
                    await IntentPnueve(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdiez":
                    await IntentPdiez(stepContext, luisResult, cancellationToken);

                    break;
                case "Ponce":
                    await IntentPonce(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdoce":
                    await IntentPdoce(stepContext, luisResult, cancellationToken);

                    break;
                case "Ptrece":
                    await IntentPtrece(stepContext, luisResult, cancellationToken);

                    break;
                case "Pcatorce":
                    await IntentPcatorce(stepContext, luisResult, cancellationToken);

                    break;
                case "Pquince":
                    await IntentPquince(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdieciseis":
                    await IntentPdieciseis(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdiecisiete":
                    await IntentPdiecisiete(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdieciocho":
                    await IntentPdieciocho(stepContext, luisResult, cancellationToken);

                    break;
                case "Pdiecinueve":
                    await IntentPdiecinueve(stepContext, luisResult, cancellationToken);

                    break;
                case "Pveinte":
                    await IntentPveinte(stepContext, luisResult, cancellationToken);

                    break;
             
                default:
                    break;

            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

       

      






        #region intentLuis
        private async Task IntentVerOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
           await stepContext.Context.SendActivityAsync("Aqui tengo mis opciones",cancellationToken:cancellationToken);
            await MainOptionsCard.ToShow(stepContext, cancellationToken);
        }

        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Hola, que gusto verte.", cancellationToken: cancellationToken);
        }

        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No te preocupes , me gusta ayudar", cancellationToken: cancellationToken);
        }

        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Espero , Verte pronto", cancellationToken: cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var resultQnA = await _qnAMakerAIServive._qnaMakerResult.GetAnswersAsync(stepContext.Context);
            var score = resultQnA.FirstOrDefault()?.Score;
            string response = resultQnA.FirstOrDefault()?.Answer;

            if (score >= 0.5)
            {
                await stepContext.Context.SendActivityAsync(response, cancellationToken: cancellationToken);

            }
            else
            {
                await stepContext.Context.SendActivityAsync("No entiendo lo que me dices", cancellationToken: cancellationToken);
                await Task.Delay(1000);
                await IntentVerOpciones(stepContext, luisResult, cancellationToken);
            }
        }
        private async Task IntentPuno(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Los coronavirus son una extensa familia de virus que pueden causar enfermedades tanto en animales como en humanos. En los humanos, se sabe que varios coronavirus causan infecciones respiratorias que pueden ir desde el resfriado común hasta enfermedades más graves como el síndrome respiratorio de Oriente Medio (MERS) y el síndrome respiratorio agudo severo (SRAS). El coronavirus que se ha descubierto más recientemente causa la enfermedad por coronavirus COVID-19.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdos(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("La COVID 19 es la enfermedad infecciosa causada por el coronavirus que se ha descubierto más recientemente. Tanto este nuevo virus como la enfermedad que provoca eran desconocidos antes de que estallara el brote en Wuhan (China) en diciembre de 2019. Actualmente la COVID 19 es una pandemia que afecta a muchos países de todo el mundo.", cancellationToken: cancellationToken);
        }

        private async Task IntentPtres(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Los síntomas más habituales de la COVID-19 son la fiebre, la tos seca y el cansancio. Otros síntomas menos frecuentes que afectan a algunos pacientes son los dolores y molestias, la congestión nasal, el dolor de cabeza, la conjuntivitis, el dolor de garganta, la diarrea, la pérdida del gusto o el olfato y las erupciones cutáneas o cambios de color en los dedos de las manos o los pies. Estos síntomas suelen ser leves y comienzan gradualmente. Algunas de las personas infectadas solo presentan síntomas levísimos.La mayoría de las personas(alrededor del 80 %) se recuperan de la enfermedad sin necesidad de tratamiento hospitalario.Alrededor de 1 de cada 5 personas que contraen la COVID 19 acaba presentando un cuadro grave y experimenta dificultades para respirar.Las personas mayores y las que padecen afecciones médicas previas como hipertensión arterial, problemas cardiacos o pulmonares, diabetes o cáncer tienen más probabilidades de presentar cuadros graves.Sin embargo, cualquier persona puede contraer la COVID 19 y caer gravemente enferma.Las personas de cualquier edad que tengan fiebre o tos y además respiren con dificultad, sientan dolor u opresión en el pecho o tengan dificultades para hablar o moverse deben solicitar atención médica inmediatamente. Si es posible, se recomienda llamar primero al profesional sanitario o centro médico para que estos remitan al paciente al establecimiento sanitario adecuado.", cancellationToken: cancellationToken);
        }
        private async Task IntentPcuatro(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("El período de incubación es el tiempo que transcurre entre la infección por el virus y la aparición de los síntomas de la enfermedad. La mayoría de las estimaciones respecto al periodo de incubación de la COVID-19 oscilan entre 1 y 14 días, y en general se sitúan en torno a 5-6 días", cancellationToken: cancellationToken);
        }
        private async Task IntentPcinco(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Una persona puede contraer la COVID 19 por contacto con otra que esté infectada por el virus. La enfermedad se propaga principalmente de persona a persona a través de las gotículas que salen despedidas de la nariz o la boca de una persona infectada al toser, estornudar o hablar. Estas gotículas son relativamente pesadas, no llegan muy lejos y caen rápidamente al suelo. Una persona puede contraer la COVID 19 si inhala las gotículas procedentes de una persona infectada por el virus. Por eso es importante mantenerse al menos a un metro de distancia de los demás. Estas gotículas pueden caer sobre los objetos y superficies que rodean a la persona, como mesas, pomos y barandillas, de modo que otras personas pueden infectarse si tocan esos objetos o superficies y luego se tocan los ojos, la nariz o la boca. Por ello es importante lavarse las manos frecuentemente con agua y jabón o con un desinfectante a base de alcohol." + 
            "La OMS está estudiando las investigaciones en curso sobre las formas de propagación de la COVID 19 y seguirá informando sobre las conclusiones que se vayan obteniendo.", cancellationToken: cancellationToken);
        }
        private async Task IntentPseis(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("La principal forma de propagación de la COVID 19 es a través de las gotículas respiratorias expelidas por alguien que tose o que tiene otros síntomas como fiebre o cansancio. Muchas personas con COVID 19 presentan solo síntomas leves. Esto es particularmente cierto en las primeras etapas de la enfermedad. Es posible contagiarse de alguien que solamente tenga una tos leve y no se sienta enfermo. Según algunas informaciones, las personas sin síntomas pueden transmitir el virus. Aún no se sabe con qué frecuencia ocurre. La OMS está estudiando las investigaciones en curso sobre esta cuestión y seguirá informando sobre las conclusiones que se vayan obteniendo.", cancellationToken: cancellationToken);
        }
        private async Task IntentPsiete(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Practicar la higiene respiratoria y de las manos es importante en TODO momento y la mejor forma de protegerse a sí mismo y a los demás." 
            +"Cuando sea posible, mantenga al menos un metro de distancia entre usted y los demás.Esto es especialmente importante si está al lado de alguien que esté tosiendo o estornudando.Dado que es posible que algunas personas infectadas aún no presenten síntomas o que sus síntomas sean leves, conviene que mantenga una distancia física con todas las personas si se encuentra en una zona donde circule el virus de la COVID 19.", cancellationToken: cancellationToken);
        }
        private async Task IntentPocho(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Si ha estado en contacto estrecho con alguien con COVID 19, puede estar infectado." 

       + "Contacto estrecho significa vivir con alguien que tiene la enfermedad o haber estado a menos de un metro de distancia de alguien que tiene la enfermedad.En estos casos, es mejor quedarse en casa."

       + "Sin embargo, si usted vive en una zona con paludismo(malaria) o dengue, es importante que no ignore la fiebre.Busque ayuda médica.Cuando acuda al centro de salud lleve mascarilla si es posible, manténgase al menos a un metro de distancia de las demás personas y no toque las superficies con las manos.En caso de que el enfermo sea un niño, ayúdelo a seguir este consejo."

        + "Si no vive en una zona con paludismo(malaria) o dengue, por favor haga lo siguiente:"
        + "Si enferma, incluso con síntomas muy leves como fiebre y dolores leves, debe aislarse en su casa."

       + " - Incluso si no cree haber estado expuesto a la COVID 19 pero desarrolla estos síntomas, aíslese y controle su estado."

        +" - Es más probable que infecte a otros en las primeras etapas de la enfermedad cuando solo tiene síntomas leves, por lo que el aislamiento temprano es muy importante."

       + "- Si no tiene síntomas pero ha estado expuesto a una persona infectada, póngase en cuarentena durante 14 días."

        + "Si ha tenido indudablemente COVID 19(confirmada mediante una prueba), aíslese durante 14 días incluso después de que los síntomas hayan desaparecido como medida de precaución.Todavía no se sabe exactamente cuánto tiempo las personas siguen siendo contagiosas después de recuperarse.Siga los consejos de las autoridades nacionales sobre el aislamiento.", cancellationToken: cancellationToken);
        }
        private async Task IntentPnueve(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("El aislamiento es una medida importante que adoptan las personas con síntomas de COVID 19 para evitar infectar a otras personas de la comunidad, incluidos sus familiares."

       +" El aislamiento se produce cuando una persona que tiene fiebre, tos u otros síntomas de COVID 19 se queda en casa y no va al trabajo, a la escuela o a lugares públicos.Lo puede hacer voluntariamente o por recomendación de su dispensador de atención de salud.Sin embargo, si vive en una zona con paludismo(malaria) o dengue, es importante que no ignore la fiebre.Busque ayuda médica.Cuando acuda al centro de salud use una mascarilla si es posible, manténgase al menos a un metro de distancia de las demás personas y no toque las superficies con las manos.En caso de que el enfermo sea un niño, ayúdelo a seguir este consejo."

        +"Si no vive en una zona con paludismo(malaria) o dengue, por favor haga lo siguiente:"
        +"Si una persona se encuentra en aislamiento, es porque está enferma pero no gravemente enferma(en cuyo caso requeriría atención médica)"
        +"- Ocupe una habitación individual amplia y bien ventilada con retrete y lavabo."

        +"- Si esto no es posible, coloque las camas al menos a un metro de distancia."

       +" - Manténgase al menos a un metro de distancia de los demás, incluso de los miembros de su familia."

       +" - Controle sus síntomas diariamente."

        +"- Aíslese durante 14 días, incluso si se siente bien."

        +"- Si tiene dificultades para respirar, póngase en contacto inmediatamente con su dispensador de atención de salud.Llame por teléfono primero si es posible."

        +"- Permanezca positivo y con energía manteniendo el contacto con sus seres queridos por teléfono o internet y haciendo ejercicio en casa.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdiez(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Las investigaciones indican que los niños y los adolescentes tienen las mismas probabilidades de infectarse que cualquier otro grupo de edad y pueden propagar la enfermedad. "

      +"  Las pruebas hasta la fecha sugieren que los niños y los adultos jóvenes tienen menos probabilidades de desarrollar una enfermedad grave, pero con todo se pueden dar casos graves en estos grupos de edad."

        +" Los niños y los adultos deben seguir las mismas pautas de cuarentena y aislamiento si existe el riesgo de que hayan estado expuestos o si presentan síntomas.Es particularmente importante que los niños eviten el contacto con personas mayores y con otras personas que corran el riesgo de contraer una enfermedad más grave.", cancellationToken: cancellationToken);
        }
        private async Task IntentPonce(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Manténgase al día de la información más reciente sobre el brote de COVID 19, a la que puede acceder en el sitio web de la OMS y a través de las autoridades de salud pública a nivel nacional y local. Se han registrado casos en la mayoría de los países del mundo, y en muchos de ellos se han producido brotes. Las autoridades de algunos países han conseguido ralentizar el avance de los brotes, pero la situación es impredecible y es necesario comprobar con regularidad las noticias más recientes."

       + " Hay varias precauciones que se pueden adoptar para reducir la probabilidad de contraer o propagar la COVID 19:"

        +"-Lávese las manos a fondo y con frecuencia usando un desinfectante a base de alcohol o con agua y jabón."
	    +"¿Por qué ? Lavarse las manos con agua y jabón o con un desinfectante a base de alcohol mata los virus que pueda haber en sus manos."

        +"- Mantenga una distancia mínima de un metro entre usted y los demás."
	  + "  ¿Por qué ? Cuando alguien tose, estornuda o habla despide por la nariz o la boca unas gotículas de líquido que pueden contener el virus.Si la persona que tose, estornuda o habla tiene la enfermedad y usted está demasiado cerca de ella, puede respirar las gotículas y con ellas el virus de la COVID 19."

      + "  - Evite ir a lugares concurridos"
      + "  ¿Por qué ? Cuando hay aglomeraciones, hay más probabilidades de que entre en contacto estrecho con alguien que tenga COVID 19 y es más difícil mantener una distancia física de un metro."

      + "  - Evite tocarse los ojos, la nariz y la boca"
      + "  ¿Por qué ? Las manos tocan muchas superficies y pueden recoger virus.Una vez contaminadas, las manos pueden transferir el virus a los ojos, la nariz o la boca.Desde allí, el virus puede entrar en su cuerpo y causarle la enfermedad."

      + "  - Tanto usted como las personas que lo rodean deben asegurarse de mantener una buena higiene respiratoria.Eso significa cubrirse la boca y la nariz con el codo flexionado o con un pañuelo al toser o estornudar.Deseche de inmediato el pañuelo usado y lávese las manos."
     + "   ¿Por qué ? Los virus se propagan a través de las gotículas.Al mantener una buena higiene respiratoria protege a las personas que lo rodean de virus como los del resfriado, la gripe y la COVID 19."

     + "   - Permanezca en casa y aíslese incluso si presenta síntomas leves como tos, dolor de cabeza y fiebre ligera hasta que se recupere.Pida a alguien que le traiga las provisiones.Si tiene que salir de casa, póngase una mascarilla para no infectar a otras personas."
     + "   ¿Por qué ? Evitar el contacto con otras personas las protegerá de posibles infecciones por el virus de la COVID 19 u otros."

     + "   - Si tiene fiebre, tos y dificultad para respirar, busque atención médica, pero en la medida de lo posible llame por teléfono con antelación y siga las indicaciones de la autoridad sanitaria local."
    + "    ¿Por qué ? Las autoridades nacionales y locales dispondrán de la información más actualizada sobre la situación en su zona.Llamar con antelación permitirá que su dispensador de atención de salud le dirija rápidamente hacia el centro de salud adecuado.Esto también lo protegerá a usted y ayudará a prevenir la propagación de virus y otras infecciones."

     + "   - Manténgase informado sobre las últimas novedades a partir de fuentes fiables, como la OMS o las autoridades sanitarias locales y nacionales."
     + "   ¿Por qué ? Las autoridades locales y nacionales son los interlocutores más indicados para dar consejos sobre lo que deben hacer las personas de su zona para protegerse.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdoce(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Aunque algunas soluciones de la medicina occidental o tradicional o remedios caseros pueden resultar reconfortantes y aliviar los síntomas leves de la COVID-19, hasta ahora ningún medicamento ha demostrado prevenir o curar esta enfermedad. La OMS no recomienda automedicarse con ningún fármaco, incluidos los antibióticos, para prevenir o curar la COVID-19. Sin embargo, hay varios ensayos clínicos en marcha, tanto de medicamentos occidentales como tradicionales. La OMS está coordinando la labor de desarrollo de vacunas y medicamentos para prevenir y tratar la COVID-19 y seguirá proporcionando información actualizada a medida que se disponga de los resultados de las investigaciones.Las formas más eficaces de protegerse a uno mismo y a los demás frente a la COVID 19 son:Lavarse las manos a fondo y con frecuencia.Evitar tocarse los ojos, la boca y la nariz.Cubrirse la boca con el codo flexionado o con un pañuelo.Si se utiliza un pañuelo, hay que desecharlo inmediatamente después de su uso y lavarse las manos.Mantener una distancia de al menos un metro con las demás personas.", cancellationToken: cancellationToken);
        }
        private async Task IntentPtrece(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("El tiempo que transcurre entre la exposición a la COVID 19 y el momento en que comienzan los síntomas suele ser de alrededor de cinco o seis días, pero puede variar entre 1 y 14 días.", cancellationToken: cancellationToken);
        }
        private async Task IntentPcatorce(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("La COVID 19 se propaga por transmisión entre seres humanos.Conocemos bastantes datos sobre otros virus de la familia de los coronavirus, y la mayoría de estos tipos de virus tienen su origen en animales.El virus de la COVID 19(también llamado SARS - CoV - 2) es un nuevo virus en los humanos.La posible fuente animal de la COVID 19 aún no ha sido confirmada, pero se está investigando.La OMS sigue monitoreando las últimas investigaciones sobre este y otros temas relacionados con la COVID 19 y proporcionará información actualizada a medida que se disponga de nuevos datos.", cancellationToken: cancellationToken);
        }
        private async Task IntentPquince(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Lo más importante que hay que saber sobre el contacto del coronavirus con superficies es que estas se pueden limpiar fácilmente con desinfectantes domésticos comunes que matarán el virus. Diversos estudios han demostrado que el virus de la COVID 19 puede sobrevivir hasta 72 horas en superficies de plástico y acero inoxidable, menos de 4 horas en superficies de cobre y menos de 24 horas en superficies de cartón.Como siempre, lávese las manos con un desinfectante a base de alcohol o con agua y jabón.Evite tocarse los ojos, la boca o la nariz.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdieciseis(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No. Los antibióticos no son eficaces contra los virus, solo contra las infecciones bacterianas. La COVID 19 está causada por un virus, de modo que los antibióticos no sirven frente a ella. No se deben usar antibióticos como medio de prevención o tratamiento de la COVID 19. En los hospitales, los médicos a veces utilizan antibióticos para prevenir o tratar infecciones bacterianas secundarias que pueden ser una complicación de la COVID 19 en pacientes gravemente enfermos. Solo deben usarse para tratar una infección bacteriana siguiendo las indicaciones de un médico.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdiecisiete(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Los principales grupos vulnerables son los mayores de 60 años, hipertensión arterial, diabetes, enfermedades cardiovasculares, enfermedades pulmonares crónicas, cáncer, inmunodeficiencias, y el embarazo por el principio de precaución.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdieciocho(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Los resultados del estudio RECOVERY muestran que la dexametasona fue capaz de reducir en aproximadamente un tercio la mortalidad entre los pacientes que necesitaban ventilación mecánica y en un quinto entre los pacientes que recibían oxígeno. Sin embargo, no se identificó ningún beneficio significativo entre aquellos pacientes que no necesitaban asistencia respiratoria, quienes seguirán siendo manejados con los tratamientos sintomáticos habituales. Tampoco se ha demostrado su eficacia en la profilaxis pre-exposición o posexposición al virus SARS-CoV-2.", cancellationToken: cancellationToken);
        }
        private async Task IntentPdiecinueve(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("La azitromicina es un antibiótico clásico en el tratamiento de las infecciones respiratorias de origen bacteriano. Es decir, en las neumonías o en las bronquitis causadas por bacterias. Se está analizando su utilización en la COVID-19 por el alto riesgo de infecciones bacterianas asociadas y por el efecto antiinflamatorio de este antibiótico.Serán las autoridades sanitarias y los profesionales quienes determinen la utilización de esta u otra opción terapéutica.En ningún caso, por supuesto, automedicarse es la opción.Y menos tratándose de un antibiótico.", cancellationToken: cancellationToken);
        }
        private async Task IntentPveinte(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Aunque estos tres síntomas son los más frecuentes, hay pacientes que no presentan síntomas o que presentan otros diferentes. Los ciudadanos sin síntomas, portadores o no, deben seguir las recomendaciones generales de prevención del contagio. En caso de aparición de tos o fiebre, estas medidas se deberían acentuar, pudiendo ser necesario el aislamiento del resto de los miembros de la familia. En estos casos se recomienda contactar con los teléfonos facilitados por las autoridades sanitarias. Si aparece dificultad respiratoria, debe llamar al teléfono habilitado por su comunidad autónoma.", cancellationToken: cancellationToken);
        }

        #endregion

        private async Task IntentVerCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Un momento por favor ...", cancellationToken:cancellationToken);
            await Task.Delay(1000);

            string idUser = stepContext.Context.Activity.From.Id;
            var medicalData = _dataBaseService.MedicalAppointment.Where(x=> x.idUser == idUser).ToList();
            if(medicalData.Count > 0)
            {
                var pending = medicalData.Where(p => p.date >= DateTime.Now.Date).ToList();
                if (pending.Count > 0)
                {
                    await stepContext.Context.SendActivityAsync("Estas son tus citas pendiente", cancellationToken: cancellationToken);
                    foreach (var item in pending)
                    {
                        await Task.Delay(1000);
                        if (item.date == DateTime.Now.Date && item.time < DateTime.Now.Hour)
                            continue;
                        string summaryMedical = $"📅 Fecha: {item.date.ToShortDateString()}" +
                            $"{Environment.NewLine}⏰ Hora: {item.time}";
                        await stepContext.Context.SendActivityAsync(summaryMedical, cancellationToken: cancellationToken);
                    }
                }
                else
                    await stepContext.Context.SendActivityAsync("😕 Lo siento pero no tienes citas pendientes", cancellationToken:cancellationToken);
            }
            else
                await stepContext.Context.SendActivityAsync("😕 Lo siento pero no tienes citas pendientes", cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCrearCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CreateAppoinmentDialog), cancellationToken: cancellationToken);
        }
        private async Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QualificationDialog), cancellationToken: cancellationToken);
        }
        private async Task IntentVerCentroContacto(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDetail = $"Nuestro numero de atencion son los siguientes:{ Environment.NewLine}" + $"📞+51 945874430{ Environment.NewLine}+📞+51 945874431";
                string addressDetail = $"🏥Estamos ubicados en{ Environment.NewLine} Calle lima 345 Ica,Ica";
            await stepContext.Context.SendActivityAsync(phoneDetail, cancellationToken:cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(addressDetail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("¿En que mas te puedo ayudar?", cancellationToken: cancellationToken);
        }


        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
        }
    }
}
