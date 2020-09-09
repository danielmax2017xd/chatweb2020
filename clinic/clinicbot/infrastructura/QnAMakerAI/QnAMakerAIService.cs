using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace clinicbot.infrastructura.QnAMakerAI
{
    public class QnAMakerAIService: IQnAMakerAIService
    {
        public QnAMaker _qnaMakerResult{ get; set; }
        public QnAMakerAIService(IConfiguration configuration)
        {
            _qnaMakerResult = new QnAMaker(new QnAMakerEndpoint { 
                
             KnowledgeBaseId = configuration["QnAmAKERBaseId"],
             EndpointKey= configuration["QnAMakerkey"],
             Host = configuration["QnAMakerHostN"],

            });
        }
    }
}
