#region Usings
#region Copyright
// (c) 2014 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Alnet.Common
{
   /// <summary>
   /// extensions for Task from System.Threading.Tasks library
   /// </summary>
   public static class TaskExtensions
   {
      /// <summary>
      /// Task.Result property throws AggregateException both on errors and for cancelled tasks, which is not suitable to pass out cancellation state or handling occured exceptions. 
      /// this method syncronousely waits for task completion and throws the same exception that underlying task including cancellation.
      /// </summary>
      /// <param name="task">task which state would be propagated as thrown exception. Can't be multiple-exception task</param>
      /// <returns>task result when there wasn't exceptions</returns>
      /// <typeparam name="T">type of task result; tasks without result are not supported</typeparam>
      /// <remarks>passing task that can cause multiple exceptions is incorrect - InvalidOperationExceptionWould be thrown</remarks>
      public static T WaitResultOrSingleException<T>(this Task<T> task)
      {
         try
         {
            return task.GetAwaiter().GetResult();
         }
         catch
         {
            if (task.IsFaulted && task.Exception.InnerExceptions != null && task.Exception.InnerExceptions.Count > 1)
            {
               //task faulted with multiple exceptions, save them and report that this is incorrect
               throw new InvalidOperationException("using WaitResultOrSingleException for task that faulted with multiple exceptions", task.Exception);
            }
            throw; //task faulted with one exception/task cancelled - rethrow
         }
      }

      /// <summary>
      /// For TaskCompletionSource-based tasks, including tasks defined in Reactive Extensions method GetAwaiter().GetResult() after cancellation throws OperationCanceledException without token
      /// this method fixes this by rethrowing exception with token
      /// </summary>
      /// <param name="originalTask">task that can be cancelled without specifiying token. Can't be multiple-exception task</param>
      /// <param name="token">token to use for cancellation</param>
      /// <returns>task whose cancellation would be marked with given token</returns>
      /// <typeparam name="T">type of task result; tasks without result are not supported</typeparam>
      public static Task<T> SingleCancellationWithToken<T>(this Task<T> originalTask, CancellationToken token)
      {
         return originalTask.ContinueWith(task =>
         {
            try
            {
               return task.WaitResultOrSingleException();
            }
            catch (Exception e)
            {
               var canceledException = e as OperationCanceledException;
               if (canceledException != null && canceledException.CancellationToken == CancellationToken.None && task.IsCanceled && token.IsCancellationRequested)
               {
                  //correspondes to situation where task was canceled by TaskCompletionSource
                  throw new OperationCanceledException("operation cancelled (rethrown with correct token)", canceledException, token);
               }
               throw; //all other exceptions are rethrown
            }
         });
      }
   }
}
