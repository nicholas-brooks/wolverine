// <auto-generated/>
#pragma warning disable

namespace Internal.Generated.WolverineHandlers
{
    // START: LetterMessage2Handler689109169
    [global::System.CodeDom.Compiler.GeneratedCode("JasperFx", "1.0.0")]
    public sealed class LetterMessage2Handler689109169 : Wolverine.Runtime.Handlers.MessageHandler
    {


        public override System.Threading.Tasks.Task HandleAsync(Wolverine.Runtime.MessageContext context, System.Threading.CancellationToken cancellation)
        {
            // The actual message body
            var letterMessage2 = (MartenTests.AggregateHandlerWorkflow.LetterMessage2)context.Envelope.Message;

            System.Diagnostics.Activity.Current?.SetTag("message.handler", "MartenTests.AggregateHandlerWorkflow.ResponseHandler");
            
            // The actual message execution
            MartenTests.AggregateHandlerWorkflow.ResponseHandler.Handle(letterMessage2);

            return System.Threading.Tasks.Task.CompletedTask;
        }

    }

    // END: LetterMessage2Handler689109169
    
    
}

