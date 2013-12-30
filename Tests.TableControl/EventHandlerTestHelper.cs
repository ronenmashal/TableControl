using System;

namespace Tests.TableControl
{
   class EventHandlerTestHelper<SenderType, EventArgsType>
         where EventArgsType : EventArgs
   {
      public bool HandlerInvoked { get; private set; }
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
         HandlerInvoked = true;
         LastInocationEventArgs = eventArgs;
         if (AdditionalHandling != null)
            AdditionalHandling(sender, eventArgs);
      }

      public void Reset()
      {
         HandlerInvoked = false;
         LastInocationEventArgs = null;
      }
   }
}