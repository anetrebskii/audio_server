using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using System.Threading;
using JetBrains.Annotations;

namespace Alnet.Common
{
   ///<summary>
   ///  Class that provides Assert and Verify methods.  These two mechanisms that can be used to provide verification
   ///  of conditions in a standard manner. The Guard.Assert method is a debug-only mechanism to check for a
   ///  condition and raise an <see cref = "InvalidOperationException" /> if the condition is not met.
   ///
   ///  Verify method                                   Assert method
   ///  RELEASE mode    throw <see cref = "InvalidOperationException" />   nothing
   ///
   ///  DEBUG mode      throw <see cref = "InvalidOperationException" />   throw <see cref = "InvalidOperationException" />
   ///
   ///
   ///  The Guard.Verify method provides the same functionality in both debug and release builds. Both should be
   ///  used extensively to ensure that the developer’s understanding of the state of the software is correct.
   ///  For example, if all the possible cases in a switch statement have been handled in code, put a Verify(false)
   ///  in the default case so that when it happens it is noticed and corrected rather than failing silently.
   ///
   ///  Verify throws <see cref = "InvalidOperationException" /> if condition is false.
   ///  Verify is always executed and is preferable to Assert unless the condition is expensive to check or the
   ///  Verify is inside a hot loop.  Try to verify all your assumptions about the executing behavior.  Lowy
   ///  suggests that every 5th line of code should be an Assert or Verify.
   ///
   ///  The first argument to Assert and Verify is a boolean that you expect to be true.  The second argument to
   ///  Assert and Verify is a string describing what went wrong.  Like in a unit test Assert, there should be enough
   ///  detail to give a developer an idea about the failure.  It is often helpful to word the message as a question.
   ///  Both Assert and Verify take insertables, so you should not use String.Format to prepare the string.  That
   ///  saves the cost of constructing the error message until the Assert/Verify has failed.
   ///  For example:
   ///  Guard.Verify(requestedSpecs.Contains(spec), "Specification must be in the hashtable of requested specs. Spec {0}.", specification);
   ///
   ///  Since Assert only happens in Debug mode, it should never be used to check the return value of a method or
   ///  anywhere else that will have side affects.  For example,
   ///  // NOTICE: improper usage of Assert, referenceCount will not be decremented in Release mode.  Should be Verify
   ///  Guard.Assert(referenceCount-- == 0, "Reference count should be back to zero.  Did you forget to call Release?");
   ///
   ///  Note: The Assert & Verify in this class interacts well with MSUnit tests.  Failed Debug.Asserts
   ///  in command line testing stop the tests from running, but in the plugin, don't even cause the test
   ///  to fail.
   ///</summary>
   [DebuggerStepThrough]
   public static class Guard
   {
      #region Public methods

      /// <summary>
      ///   A Elvees equivalent of Assert.
      ///   This is a DEBUG-only method, all calls will be compiled out in release mode builds.
      ///   Therefore, calls to it MUST not have side effects.
      ///   Throws <see cref = "InvalidOperationException" /> exception if the condition is not met.
      /// </summary>
      /// <param name = "condition">True to prevent the assert action, otherwise - false.</param>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      [Conditional("DEBUG")]
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)]bool condition, string messageWithInsertables, params object[] args)
      {
         Verify(condition, messageWithInsertables, args);
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   This method calls Assert method with condition=false
      /// </summary>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      [Conditional("DEBUG")]
      [ContractArgumentValidator]
      public static void Fail(string messageWithInsertables, params object[] args)
      {
         Assert(false, messageWithInsertables, args);
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   An equivalent of Assert that executes in Release and Debug mode.
      ///   It throws <see cref = "InvalidOperationException" /> if condition is false.
      /// </summary>
      /// <param name = "condition">True to prevent a message being displayed, otherwise - false.</param>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void Verify([AssertionCondition(AssertionConditionType.IS_TRUE)]bool condition, string messageWithInsertables, params object[] args)
      {
         if (!condition)
         {
            string message = buildMessageText(messageWithInsertables, args);
            throw new InvalidOperationException(message);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "NullReferenceException" /> when a checked object is <see langword = "null" />.
      /// </summary>
      /// <param name = "obj">Object to verify.</param>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      /// <remarks>
      ///   This is a DEBUG-only method.
      /// </remarks>
      [Conditional("DEBUG")]
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void AssertNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]object obj, string messageWithInsertables, params object[] args)
      {
         VerifyNotNull(obj, messageWithInsertables, args);
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "NullReferenceException" /> when a checked object is <see langword = "null" />.
      /// </summary>
      /// <param name = "obj">Object to verify.</param>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]object obj, string messageWithInsertables, params object[] args)
      {
         if (obj == null)
         {
            string message = buildMessageText(messageWithInsertables, args);
            throw new NullReferenceException(message);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "InvalidOperationException" /> when a checked object is not <see langword = "null" />.
      /// </summary>
      /// <param name = "obj">Object to verify.</param>
      /// <param name = "messageWithInsertables">Description of the failed condition.</param>
      /// <param name = "args">Arguments which will be inserted to the description.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyIsNull([AssertionCondition(AssertionConditionType.IS_NULL)]object obj, string messageWithInsertables, params object[] args)
      {
         if (obj != null)
         {
            string message = buildMessageText(messageWithInsertables, args);
            throw new InvalidOperationException(message);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "ArgumentException" /> when a condition related to a method argument is not met.
      /// </summary>
      /// <param name = "argumentCondition">The condition that must be fulfilled, or otherwise the method will throw.</param>
      /// <param name = "messageWithInsertables">The message of the exception.</param>
      /// <param name = "args">The optional argument list for the message.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgument([AssertionCondition(AssertionConditionType.IS_TRUE)]bool argumentCondition, string messageWithInsertables, params object[] args)
      {
         if (!argumentCondition)
         {
            string message = buildMessageText(messageWithInsertables, args);
            throw new ArgumentException(message);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "ArgumentNullException" /> when a method argument is null.
      /// </summary>
      /// <param name = "argument">The argument being checked for null.</param>
      /// <param name = "argumentName">The argument name.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgumentNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]object argument, string argumentName)
      {
         debugVerifyArgumentName(argumentName);
         if (argument == null)
         {
            throw new ArgumentNullException(argumentName);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///  Throws an <see cref = "InvalidOperationException" /> when "time" isn't UTC time.
      /// </summary>
      /// <param name = "time">Time to check kind..</param>
      /// <param name = "argumentName">The argument name.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgumentIsUtc(DateTime time, string argumentName)
      {
         debugVerifyArgumentName(argumentName);
         if (!Guard.IsUtc(time))
         {
            throw new ArgumentException("Time is not in UTC format", argumentName);
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      /// Checks "time" parameter that is UTC time.
      /// </summary>
      /// <param name = "time">Time to check kind..</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      /// <returns>true if  time is UTC</returns>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static bool IsUtc(DateTime time)
      {
         return time.Kind == DateTimeKind.Utc;
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "ArgumentNullException" /> when a method argument (of type String) is null or empty.
      /// </summary>
      /// <param name = "argument">The argument being checked.</param>
      /// <param name = "argumentName">The argument name.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgumentNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]string argument, string argumentName)
      {
         debugVerifyArgumentName(argumentName);
         if (String.IsNullOrEmpty(argument))
         {
            throw new ArgumentNullException(argumentName, "Value cannot be null or empty.");
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "ArgumentNullException" /> when a method argument (of type String) is null or empty or contains only white chars.
      /// </summary>
      /// <param name = "argument">The argument being checked.</param>
      /// <param name = "argumentName">The argument name.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgumentNotNullOrWhiteSpace([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]string argument, string argumentName)
      {
         debugVerifyArgumentName(argumentName);
         if (String.IsNullOrWhiteSpace(argument))
         {
            throw new ArgumentNullException(argumentName, "Value cannot be null or empty.");
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Throws an <see cref = "ArgumentOutOfRangeException" /> when the value of the argument is outside
      ///   the allowable range of values.
      /// </summary>
      /// <param name = "argument">The argument being checked.</param>
      /// <param name = "minimum">Minimum allowed value (inclusive).</param>
      /// <param name = "maximum">Maximum allowed value (inclusive).</param>
      /// <param name = "argumentName">The argument name.</param>
      /// <remarks>
      ///   This check is performed both in DEBUG and RELEASE.
      /// </remarks>
      [ContractArgumentValidator]
      [AssertionMethod]
      public static void VerifyArgumentInRange(object argument, object minimum, object maximum, string argumentName)
      {
         debugVerifyArgumentName(argumentName);
         var comparableArgument = argument as IComparable;
         var comparableMin = minimum as IComparable;
         var comparableMax = maximum as IComparable;
         if (comparableArgument == null)
         {
            throw new ArgumentException("Argument value must be IComparable", "argument");
         }
         if (comparableMin == null)
         {
            throw new ArgumentException("Minimum value must be IComparable", "minimum");
         }
         if (comparableMax == null)
         {
            throw new ArgumentException("Maximum value must be IComparable", "maximum");
         }
         if (comparableArgument.CompareTo(comparableMin) < 0 || comparableArgument.CompareTo(comparableMax) > 0)
         {
            throw new ArgumentOutOfRangeException(argumentName, argument,
                                                  string.Format("Argument {0} must be in range [{1}-{2}].", argumentName,
                                                                minimum, maximum));
         }
         Contract.EndContractBlock();
      }

      ///<summary>
      ///  Verifies that the passed argument is not null and returns it.
      ///  This method has no argument representing name because is should be used
      ///  only for cases when instance name is meaningless and only type is enough to determine exact argument - 
      ///  like it nearly always for components used in class,
      ///  because class uses exactly one instance of a component.
      ///</summary>
      ///<typeparam name = "T">Type of a argument.</typeparam>
      ///<param name = "arg">The argument.</param>
      ///<returns>The passed argument.</returns>
      ///<remarks>
      ///  This method is useful when calling base class constructor and
      ///  need to access constructor parameter fields.
      ///</remarks>
      ///<example>
      ///  This sample show how to use this method.
      ///  <code>
      ///    class S
      ///    {
      ///    int ID;
      ///    public S(int id)
      ///    {
      ///    this.ID = id;
      ///    }
      ///    }
      ///
      ///    class Class1
      ///    {
      ///    int _id;
      ///    public Class1(int id) { this._id = id; }
      ///    }
      ///
      ///    class Class2 : Class1
      ///    {
      ///    public Class2(S s) : base(Guard.EnsureArgumentNotNull(s).ID) { }
      ///    }
      ///  </code>
      ///</example>
      [ContractArgumentValidator]
      [AssertionMethod]
      [NotNull]
      public static T EnsureArgumentNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]T arg)
      {
         if (arg == null)
         {
            throw new ArgumentNullException(typeof(T).AssemblyQualifiedName, "argument instance of given type is null");
         }
         Contract.EndContractBlock();
         return arg;
      }

      /// <summary>
      ///   Verifies that the passed argument is not null and returns it.
      /// </summary>
      /// <typeparam name = "T"></typeparam>
      /// <param name = "arg">The argument.</param>
      /// <param name = "argumentName">Name of the argument.</param>
      /// <returns>The passed argument.</returns>
      /// <remarks>
      ///   This method is useful when calling base class constructor and
      ///   need to access constructor parameter fields.
      /// </remarks>
      /// <example>
      ///   This sample show how to use this method.
      ///   <code>
      ///     class S
      ///     {
      ///     int ID;
      ///     public S(int id)
      ///     {
      ///     this.ID = id;
      ///     }
      ///     }
      ///     class Class1
      ///     {
      ///     int _id;
      ///     public Class1(int id) { this._id = id; }
      ///     }
      ///     class Class2 : Class1
      ///     {
      ///     public Class2(S s) : base(Guard.EnsureArgumentNotNull(s).ID) { }
      ///     }
      ///   </code>
      /// </example>
      [ContractArgumentValidator]
      [AssertionMethod]
      [NotNull]
      public static T EnsureArgumentNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]T arg, string argumentName) where T : class
      {
         VerifyArgumentNotNull(arg, argumentName);
         Contract.EndContractBlock();
         return arg;
      }

      /// <summary>
      ///   Verifies that the passed argument is not null or empty and returns it.
      /// </summary>
      /// <param name = "arg">The argument.</param>
      /// <param name = "argumentName">Name of the argument.</param>
      /// <returns>The passed argument.</returns>
      /// <remarks>
      ///   This method is useful to make inline arguments check and set.
      /// </remarks>
      /// <example>
      ///   This sample show how to use this method.
      ///   <code>
      ///     class Class1
      ///     {
      ///     string _name;
      /// 
      ///     public Class1(string name) 
      ///     { 
      ///     _name = Guard.EnsureArgumentNotNullOrEmpty(name, "name"); 
      ///     }
      ///     }
      ///   </code>
      /// </example>
      [ContractArgumentValidator]
      [AssertionMethod]
      [NotNull]
      public static string EnsureArgumentNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]string arg, string argumentName)
      {
         VerifyArgumentNotNullOrEmpty(arg, argumentName);
         Contract.EndContractBlock();
         return arg;
      }

      /// <summary>
      ///   Verifies that the passed argument is not null or empty or contains only white-space characters and returns it.
      /// </summary>
      /// <param name = "arg">The argument.</param>
      /// <param name = "argumentName">Name of the argument.</param>
      /// <returns>The passed argument.</returns>
      /// <remarks>
      ///   This method is useful to make inline arguments check and set.
      /// </remarks>
      /// <example>
      ///   This sample show how to use this method.
      ///   <code>
      ///     class Class1
      ///     {
      ///     string _name;
      /// 
      ///     public Class1(string name) 
      ///     { 
      ///     _name = Guard.EnsureArgumentNotNullOrWhiteSpace(name, "name"); 
      ///     }
      ///   </code>
      /// </example>
      [ContractArgumentValidator]
      [AssertionMethod]
      [NotNull]
      public static string EnsureArgumentNotNullOrWhiteSpace([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]string arg, string argumentName)
      {
         VerifyArgumentNotNullOrWhiteSpace(arg, argumentName);
         Contract.EndContractBlock();
         return arg;
      }

      /// <summary>
      ///   Verifies that the passed value is not null and returns it.
      /// </summary>
      /// <typeparam name = "T"></typeparam>
      /// <param name = "arg">The argument.</param>
      /// <param name = "messageWithInsertables">The message with insertables.</param>
      /// <param name = "args">The args.</param>
      /// <returns>The passed argument.</returns>
      /// <remarks>
      ///   This method is useful when calling base class constructor and
      ///   need to access constructor parameter fields.
      /// </remarks>
      /// <example>
      ///   This sample show how to use this method.
      ///   <code>
      ///     class S
      ///     {
      ///     string ID;
      ///     public S(string id)
      ///     {
      ///     this.ID = id;
      ///     }
      ///     }
      ///     class Class1
      ///     {
      ///     string _id;
      ///     public Class1(string id) { this._id = id; }
      ///     }
      ///     class Class2 : Class1
      ///     {
      ///     public Class2(S s) : base(Guard.EnsureValueNotNull(s, "Value ID of class '{0}' is null", s).ID) { }
      ///     }
      ///   </code>
      /// </example>
      [ContractArgumentValidator]
      [AssertionMethod]
      [StringFormatMethod("messageWithInsertables")]
      [NotNull]
      public static T EnsureValueNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]T arg, string messageWithInsertables, params object[] args) where T : class
      {
         Verify(arg != null, messageWithInsertables, args);
         Contract.EndContractBlock();
         return arg;
      }

      /// <summary>
      ///   Verifies that the passed nullable value is not null and returns it's value.
      /// </summary>
      /// <typeparam name = "T">type of value inside nullable</typeparam>
      /// <param name = "arg">The argument.</param>
      /// <param name = "messageWithInsertables">The message with insertables.</param>
      /// <param name = "args">Message parameters.</param>
      /// <returns>The <see cref="Nullable{T}.Value"/> property of passed argument.</returns>
      [ContractArgumentValidator]
      [AssertionMethod]
      [StringFormatMethod("messageWithInsertables")]
      public static T EnsureValueNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]T? arg, string messageWithInsertables, params object[] args) where T : struct
      {
         Verify(arg != null, messageWithInsertables, args);
         Contract.EndContractBlock();
         return arg.Value;
      }
      
      /// <summary>
      ///   Determines whether the specified exception is fatal.
      /// </summary>
      /// <remarks>
      ///   <see cref = "StackOverflowException" /> is automatically rethrow at the end of the catch block,
      ///   but this method ensures that it's re thrown immediately.
      ///   <see cref = "StackOverflowException" /> is not caught by the catch block since .Net Framework 2.0,
      ///   but this method contains it for the best reliability.
      /// </remarks>
      /// <param name = "ex">The exception.</param>
      /// <returns><c>true</c> if the specified ex is fatal; otherwise, <c>false</c>.</returns>
      [ContractArgumentValidator]
      public static bool IsFatal(Exception ex)
      {
         return (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException);
      }

      /// <summary>
      ///   Re throws the exception if it is fatal.
      /// </summary>
      /// <remarks>
      ///   <see cref = "StackOverflowException" /> is automatically rethrow at the end of the catch block,
      ///   but this method ensures that it's re thrown immediately.
      ///   <see cref = "StackOverflowException" /> is not caught by the catch block since .Net Framework 2.0,
      ///   but this method contains it for the best reliability.
      /// </remarks>
      /// <param name = "ex">The exception.</param>
      public static void RethrowIfFatal(Exception ex)
      {
         if (ex is OutOfMemoryException)
         {
            ExceptionDispatchInfo.Capture(ex).Throw();
         }

         if (ex is StackOverflowException)
         {
            ExceptionDispatchInfo.Capture(ex).Throw();
         }
      }

      /// <summary>
      ///   Re throws the exception if it is fatal or it is <see cref="OperationCanceledException"/> or its derives.
      /// </summary>
      /// <param name = "ex">The exception.</param>
      public static void RethrowIfFatalOrCancelled(Exception ex)
      {
         RethrowIfFatal(ex);

         if (ex is OperationCanceledException)
         {
            ExceptionDispatchInfo.Capture(ex).Throw();
         }
      }

      /// <summary>
      ///   Casts the specified instance of the <typeparam name = "TInterface" /> to the <typeparam name = "TInstance" />.
      /// </summary>
      /// <param name = "instance">The instance to cast.</param>
      /// <returns>The casted instance.</returns>
      /// <exception cref = "ArgumentNullException">Instance to cast is <see langword = "null" />.</exception>
      /// <exception cref = "ArgumentException">Instance is not of the type <typeparamref name = "TInstance" />.</exception>
      /// <remarks>
      ///   This method is useful when some functionality expects specific 
      ///   type passed to it as interface.
      /// </remarks>
      [ContractArgumentValidator]
      public static TInstance Cast<TInterface, TInstance>(TInterface instance) where TInstance : TInterface
      {
         VerifyArgumentNotNull(instance, "instance");
         VerifyArgument(instance is TInstance, "The instance is not compatible with given type.");
         Contract.EndContractBlock();
         return (TInstance)instance;
      }

      #endregion

      #region Private methods

      /// <summary>
      ///   Will not be called in RELEASE mode.
      /// </summary>
      [Conditional("DEBUG")]
      [ContractAbbreviator]
      private static void debugVerifyArgumentName(string argumentName)
      {
         if (string.IsNullOrEmpty(argumentName))
         {
            throw new ArgumentNullException("argumentName", "The argumentName parameter must not be null or empty.");
         }
         Contract.EndContractBlock();
      }

      /// <summary>
      ///   Builds the exception message text.
      /// </summary>
      /// <param name = "messageWithInsertables">The exception message template.</param>
      /// <param name = "args">The arguments list.</param>
      /// <returns>Returns the built message text.</returns>
      [ContractAbbreviator]
      private static string buildMessageText(string messageWithInsertables, object[] args)
      {
         if (String.IsNullOrEmpty(messageWithInsertables))
         {
            throw new ArgumentException(
               "The error message parameter should describe the problem if the condition is not met.");
         }
         Contract.EndContractBlock();
         return String.Format(messageWithInsertables, args);
      }

      #endregion
   }
}