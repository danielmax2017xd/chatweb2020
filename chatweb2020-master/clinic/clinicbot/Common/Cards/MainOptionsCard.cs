using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace clinicbot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task ToShow(DialogContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(activity: CreatoCarousel(), cancellationToken);

        }
        private static Activity CreatoCarousel()
        {
            var cardTestCovid = new HeroCard
            {
                Title = "Test Covid",
                Subtitle = "opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstorage1.blob.core.windows.net/images/online-doctor-with-computer_23-2148525681.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction()  {Title="Test Virtual" , Value = "Test Virtual", Type=ActionTypes.ImBack},
                    // new CardAction()  {Title="Ver mi cita" , Value = "Ver mi cita", Type=ActionTypes.ImBack},

                }
            };
            var cardCitaMedica = new HeroCard
            {
                Title = "Citas Medicas",
                Subtitle = "opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstorage1.blob.core.windows.net/images/menu_01.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction()  {Title="Crear cita medica" , Value = "Crear cita medica", Type=ActionTypes.ImBack},
                     new CardAction()  {Title="Ver mi cita" , Value = "Ver mi cita", Type=ActionTypes.ImBack},

                }
            };

            var cardInformacionContacto = new HeroCard
            {
                Title = "Centro de contacto",
                Subtitle = "opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstorage1.blob.core.windows.net/images/menu_02.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction()  {Title="Centro de contacto" , Value = "Centro de contacto", Type=ActionTypes.ImBack},
                     new CardAction()  {Title="Sitio web" , Value = "WEB", Type=ActionTypes.OpenUrl},

                }
            };
        
            var cardSiguenosRedes = new HeroCard
            {
                Title = "Siguenos en redes",
                Subtitle = "opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstorage1.blob.core.windows.net/images/menu_03.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction()  {Title="Facebook" , Value = "https://www.facebook.com/", Type=ActionTypes.OpenUrl},
                     new CardAction()  {Title="Instagram" , Value = "https://www.facebook.com/", Type=ActionTypes.OpenUrl},
                      new CardAction()  {Title="Twuiter" , Value = "https://www.facebook.com/", Type=ActionTypes.OpenUrl},
                      
                }
            };
            var cardCalificacion = new HeroCard
            {
                Title = "Calificacion",
                Subtitle = "opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstorage1.blob.core.windows.net/images/menu_04.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction()  {Title="Calificar Bot" , Value = "Calificar Bot", Type=ActionTypes.ImBack},
                    

                }
            };
            var optionsAttachments = new List<Attachment>()
            {
                cardTestCovid.ToAttachment(),
              cardCitaMedica.ToAttachment(),
              cardInformacionContacto.ToAttachment(),
           
              cardSiguenosRedes.ToAttachment(),
              cardCalificacion.ToAttachment(),

            };
            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;


        
        }
    }

}
