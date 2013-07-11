using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace _DGTester.Validation
{
   public class ExtendedValidationResult : ValidationResult
   {
      public static readonly ExtendedValidationResult Valid = new ExtendedValidationResult(ResultType.Valid, string.Empty, ValidationActions.None);

      private ExtendedValidationResult(ResultType result, object errorContent, ValidationActions actions) :
         base(result == ResultType.Valid, errorContent)
      {
         Result = result;
         Actions = actions;
      }

      public ResultType Result
      {
         get;
         private set;
      }

      public ValidationActions Actions
      {
         get;
         private set;
      }

      public string Message
      {
         get { return ErrorContent as string; }
      }

      public bool ShouldBlock
      {
         get { return Actions.HasFlag(ValidationActions.Block); }
      }

      public bool ShouldShowIndication
      {
         get { return Actions.HasFlag(ValidationActions.ShowIndication); }
      }

      public override string ToString()
      {
         return Result.ToString();
      }

      public static ExtendedValidationResult Warning(string message, ValidationActions actions = ValidationActions.ShowIndication)
      {
         return new ExtendedValidationResult(ResultType.Warning, message, actions);
      }

      public static ExtendedValidationResult Error(string message,
                                                   ValidationActions actions = (ValidationActions.Block | ValidationActions.ShowIndication))
      {
         return new ExtendedValidationResult(ResultType.Error, message, actions);
      }

      public static ExtendedValidationResult MergeResults(IEnumerable<ExtendedValidationResult> results)
      {
         if (results == null)
         {
            throw new ArgumentNullException("results");
         }

         if (results.Count() == 0)
         {
            return Valid;
         }

         var messages = new List<string>();
         var resultType = ResultType.Valid;
         var action = ValidationActions.None;

         foreach (var result in results)
         {
            var message = result.Message;
            if (!string.IsNullOrEmpty(message))
            {
               messages.Add(message);
            }

            if (result.Result > resultType)
            {
               resultType = result.Result;
            }

            if (!action.HasFlag(result.Actions))
            {
               action |= result.Actions;
            }
         }

         string msg;
         if (messages.Count == 0)
         {
            msg = string.Empty;
         }
         else
         {
            msg = string.Join(Environment.NewLine, messages);
         }

         return new ExtendedValidationResult(resultType, msg, action);
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(obj, this))
         {
            return true;
         }
         var result = obj as ExtendedValidationResult;
         if (result == null)
         {
            return false;
         }
         return ((Result == result.Result) && (Message == result.Message) && (Actions == result.Actions));
      }

      public static bool operator ==(ExtendedValidationResult left, ExtendedValidationResult right)
      {
         return Equals(left, right);
      }

      public static bool operator !=(ExtendedValidationResult left, ExtendedValidationResult right)
      {
         return !Equals(left, right);
      }

      public bool Equals(ExtendedValidationResult result)
      {
         if (ReferenceEquals(null, result)) return false;
         if (ReferenceEquals(this, result)) return true;

         return ((Result == result.Result) && (Message == result.Message) && (Actions == result.Actions));
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int result = Result.GetHashCode();
            result = (result * 397) ^ Message.GetHashCode();
            result = (result * 397) ^ Actions.GetHashCode();
            return result;
         }
      }
   }
}
