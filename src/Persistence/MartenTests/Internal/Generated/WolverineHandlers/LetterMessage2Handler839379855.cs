// <auto-generated/>

using MartenTests.AggregateHandlerWorkflow;

#pragma warning disable

namespace Internal.Generated.WolverineHandlers
{
    // START: LetterMessage2Handler839379855
    public class LetterMessage2Handler839379855 : Wolverine.Runtime.Handlers.MessageHandler
    {


        public override System.Threading.Tasks.Task HandleAsync(Wolverine.Runtime.MessageContext context, System.Threading.CancellationToken cancellation)
        {
            // The actual message body
            var letterMessage2 = (LetterMessage2)context.Envelope.Message;

            
            // The actual message execution
            ResponseHandler.Handle(letterMessage2);

            return System.Threading.Tasks.Task.CompletedTask;
        }

    }

    // END: LetterMessage2Handler839379855
    
    
}

