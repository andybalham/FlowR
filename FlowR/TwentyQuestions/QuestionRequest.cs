using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class QuestionRequest : FlowActivityRequest<QuestionResponse>
    {
        public static FlowActivityDefinition<QuestionRequest, QuestionResponse> NewDefinition() => 
            new FlowActivityDefinition<QuestionRequest, QuestionResponse>();

        public override string GetText() => $"Ask '{Question}'";

        [NotNullValue]
        public string Question { get; set; }

        [NotNullValue]
        public string[] Answers { get; set; }
    }

    public class QuestionResponse
    {
        public string Answer { get; set; }
    }

    public class QuestionHandler : IRequestHandler<QuestionRequest, QuestionResponse>
    {
        public Task<QuestionResponse> Handle(QuestionRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine(request.Question);
            Console.WriteLine(string.Join(", ", request.Answers));

            var options =
                request.Answers.Select(a =>
                        Regex.Match(a, "\\[(?<answer>[^]])\\]").Groups["answer"].Value.ToUpper())
                    .ToHashSet();

            var answer = Console.ReadLine()?.ToUpper();

            while (!options.Contains(answer))
            {
                Console.WriteLine(string.Join(", ", request.Answers));
                answer = Console.ReadLine()?.ToUpper();
            }

            Console.WriteLine("****************************************************");

            return Task.FromResult(new QuestionResponse { Answer = answer });
        }
    }
}
