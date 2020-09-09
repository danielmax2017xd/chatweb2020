// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using clinicbot.Common.Models.User;
using clinicbot.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace clinicbot
{
    public class ClinicBot <T> : ActivityHandler where T: Dialog
    {
        // protected readonly Dialog dialog;
        //    protected readonly BotState _conversationState;


        private readonly BotState _userState;
        private readonly BotState _conversationState;
        private readonly Dialog _dialog;
        private readonly ILogger _logger;

        private readonly IDataBaseService _dataBaseService;

      //  public ClinicBot(T dialog, ConversationState conversationState , ILogger<ClinicBot<T>>logger)
        //{
          //  _dialog = dialog;
            //_conversationState = conversationState;
           // _logger = logger;
        //}
        public ClinicBot(UserState userState, ConversationState conversationState , T dialog, IDataBaseService dataBaseService , ILogger<ClinicBot<T>> logger)
        {
           // _userState = userState;
          //  _conversationState = conversationState;
            _dialog = dialog;
            _conversationState = conversationState;
            _userState = userState;
            _logger = logger;
            _dataBaseService = dataBaseService;
        }
        //  public ClinicBot(T dialog, ConversationState conversationState , ILogger<ClinicBot<T>>logger)
        //{
        //  _dialog = dialog;
        //_conversationState = conversationState;
        // _logger = logger;
        //}
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                   await turnContext.SendActivityAsync(MessageFactory.Text($"Hola soy tu Doctor virtual 🤖 "), cancellationToken);
                   await turnContext.SendActivityAsync(MessageFactory.Text($"¿Me podrias indicar tu nombre? para dirigirme a usted"), cancellationToken);
                }
            }
        }
      

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _userState.SaveChangesAsync(turnContext,false, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);

        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            await SaveUser(turnContext);

            // var userMessage = turnContext.Activity.Text

            //await turnContext.SendActivityAsync($"User: {userMessage}" ,  cancellationToken: cancellationToken);
            await _dialog.RunAsync(
                turnContext,
                _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                cancellationToken
                );

        }

        private async Task SaveUser(ITurnContext<IMessageActivity> turnContext)
        {
            var userModel = new UserModel();
            userModel.id = turnContext.Activity.From.Id;
            userModel.userNameChannle = turnContext.Activity.From.Name;
            userModel.channel = turnContext.Activity.ChannelId;
            userModel.registerDate = DateTime.Now.Date;

            var user = await _dataBaseService.User.FirstOrDefaultAsync(x => x.id == turnContext.Activity.From.Id);
            if (user==null){
                await _dataBaseService.User.AddAsync(userModel);
                await _dataBaseService.SaveAsync();

            }
           }
    }
}
