﻿using clinicbot.Common.Models.Qualificacion;
using clinicbot.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace clinicbot.Dialogs.Qualificacion
{
    public class QualificationDialog:ComponentDialog
    {
        private IDataBaseService _dataBaseService;
        public QualificationDialog(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
            var waterfallSteps = new WaterfallStep[]
            {
                ToShowButton,
                ValidateOption
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog),waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        private async Task<DialogTurnResult> ToShowButton(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                { 
                    Prompt=CreateButtonsQualification()
                    
                },
                cancellationToken
                
             );
        }

        private Activity CreateButtonsQualification()
        {
            var reply = MessageFactory.Text("Calificame por favor");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title="1⭐",Value="1⭐", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="2⭐",Value="2⭐", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="3⭐",Value="3⭐", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="4⭐",Value="4⭐", Type=ActionTypes.ImBack},
                    new CardAction(){ Title="5⭐",Value="5⭐", Type=ActionTypes.ImBack},
                }
            };
            return reply as Activity;
    }
        private async Task<DialogTurnResult> ValidateOption(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Context.Activity.Text;
            await stepContext.Context.SendActivityAsync($"Gracias por tu {options}",cancellationToken:cancellationToken);
            await Task.Delay(1000);
            //GUARDAR CALIFICACION
            await SaveQualification(stepContext, options);
            await stepContext.Context.SendActivityAsync("¿En que mas te puedo ayudar?",cancellationToken:cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task SaveQualification(WaterfallStepContext stepContext, string options)
        {
            var qualificationModel = new QualificationModel();
            qualificationModel.id = Guid.NewGuid().ToString();
            qualificationModel.idUser = stepContext.Context.Activity.From.Id;
            qualificationModel.qualification = options;
            qualificationModel.registerDate = DateTime.Now.Date;

            await _dataBaseService.Qualification.AddAsync(qualificationModel);
            await _dataBaseService.SaveAsync();
        }
    }
}
