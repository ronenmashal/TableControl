using System;

namespace Tests.TableControl
{
   class EventHandlerTestHelper<SenderType, EventArgsType>
         where EventArgsType : EventArgs
   {
      public int HandlerInvocationCount { get; private set; }
      public bool HandlerInvoked { get { return HandlerInvocationCount > 0; } }
      public EventArgsType LastInocationEventArgs { get; private set; }
      public EventHandler<EventArgsType> AdditionalHandling { get; set; }

      private string eventName;

      public EventHandlerTestHelper(string eventName)
      {
         this.eventName = eventName;
         AdditionalHandling = null;
      }

      public void Handler(SenderType sender, EventArgsType eventArgs)
      {
         HandlerInvocationCount++;
         LastInocationEventArgs = eventArgs;
         if (AdditionalHandling != null)
            AdditionalHandling(sender, eventArgs);
      }

      public void Reset()
      {
         HandlerInvocationCount = 0;
         LastInocationEventArgs = null;
      }
   }
}