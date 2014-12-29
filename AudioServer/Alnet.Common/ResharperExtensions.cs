// Extracted from Re-Sharper.Annotations.dll
// Used for code annotations to works with re-sharper.

using System;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
namespace JetBrains.Annotations
// ReSharper restore CheckNamespace
{
   /// <summary>
   /// Indicates that the value of marked element could be <c>null</c> sometimes, so the check for <c>null</c> is necessary before its usage.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
   public sealed class CanBeNullAttribute : Attribute
   {
   }

   /// <summary>
   /// Indicates that the value of marked element could never be <c>null</c>.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
   public sealed class NotNullAttribute : Attribute
   {
   }

   /// <summary>
   /// Indicates that the method is contained in a type that implements
   /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface
   /// and this method is used to notify that some property value changed
   /// </summary>
   /// <remarks>
   /// The method should be non-static and conform to one of the supported signatures:
   /// <list>
   /// <item><c>NotifyChanged(string)</c></item>
   /// <item><c>NotifyChanged(params string[])</c></item>
   /// <item><c>NotifyChanged{T}(Expression{Func{T}})</c></item>
   /// <item><c>NotifyChanged{T,U}(Expression{Func{T,U}})</c></item>
   /// <item><c>SetProperty{T}(ref T, T, string)</c></item>
   /// </list>
   /// </remarks>
   /// <example><code>
   /// public class Foo : INotifyPropertyChanged {
   ///   public event PropertyChangedEventHandler PropertyChanged;
   ///   [NotifyPropertyChangedInvocator]
   ///   protected virtual void NotifyChanged(string propertyName) { ... }
   ///
   ///   private string _name;
   ///   public string Name {
   ///     get { return _name; }
   ///     set { _name = value; NotifyChanged("LastName"); /* Warning */ }
   ///   }
   /// }
   /// </code>
   /// Examples of generated notifications:
   /// <list>
   /// <item><c>NotifyChanged("Property")</c></item>
   /// <item><c>NotifyChanged(() =&gt; Property)</c></item>
   /// <item><c>NotifyChanged((VM x) =&gt; x.Property)</c></item>
   /// <item><c>SetProperty(ref myField, value, "Property")</c></item>
   /// </list>
   /// </example>
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
   public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
   {
      public NotifyPropertyChangedInvocatorAttribute() { }
      public NotifyPropertyChangedInvocatorAttribute(string parameterName)
      {
         ParameterName = parameterName;
      }

      public string ParameterName { get; private set; }
   }

   /// <summary>
   ///  Indicates that the marked method is assertion method, i.e. it halts control flow if one of the conditions is satisfied. 
   ///  To set the condition, mark one of the parameters with <see cref="T:JetBrains.Annotations.AssertionConditionAttribute"/> attribute
   /// </summary>
   /// <seealso cref="T:JetBrains.Annotations.AssertionConditionAttribute"/>
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
   public sealed class AssertionMethodAttribute : Attribute
   {
   }

   /// <summary>
   /// Specifies assertion type. If the assertion method argument satisfies the condition, then the execution continues. 
   /// Otherwise, execution is assumed to be halted
   /// </summary>
   public enum AssertionConditionType
   {
      /// <summary>
      /// Indicates that the marked parameter should be evaluated to true.
      /// </summary>
      IS_TRUE,

      /// <summary>
      /// Indicates that the marked parameter should be evaluated to false.
      /// </summary>
      IS_FALSE,

      /// <summary>
      /// Indicates that the marked parameter should be evaluated to null value.
      /// </summary>
      IS_NULL,

      /// <summary>
      /// Indicates that the marked parameter should be evaluated to not null value.
      /// </summary>
      IS_NOT_NULL,
   }

   /// <summary>
   /// Indicates the condition parameter of the assertion method. 
   /// The method itself should be marked by <see cref="T:JetBrains.Annotations.AssertionMethodAttribute"/> attribute.
   /// The mandatory argument of the attribute is the assertion type.
   ///</summary>
   /// <seealso cref="T:JetBrains.Annotations.AssertionConditionType"/>
   [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
   public sealed class AssertionConditionAttribute : Attribute
   {
      /// <summary>
      /// Gets condition type.
      /// </summary>
      public AssertionConditionType ConditionType { get; private set; }

      /// <summary>
      /// Initializes new instance of AssertionConditionAttribute.
      /// </summary>
      /// <param name="conditionType">Specifies condition type.</param>
      public AssertionConditionAttribute(AssertionConditionType conditionType)
      {
         ConditionType = conditionType;
      }
   }

   /// <summary>
   /// Indicates that the function argument should be string literal and match one  of the parameters of the caller function.
   /// For example, <see cref="T:System.ArgumentNullException"/> has such parameter.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
   public sealed class InvokerParameterNameAttribute : Attribute
   {
   }

   /// <summary>
   /// Indicates that marked method builds string by format pattern and (optional) arguments. 
   /// Parameter, which contains format string, should be given in constructor.
   /// The format string should be in <see cref="M:System.String.Format(System.IFormatProvider,System.String,System.Object[])"/> -like form
   /// </summary>
   [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
   public sealed class StringFormatMethodAttribute : Attribute
   {
      /// <summary>
      /// Gets format parameter name.
      /// </summary>
      public string FormatParameterName { get; private set; }

      /// <summary>
      /// Initializes new instance of StringFormatMethodAttribute.
      /// </summary>
      /// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string.</param>
      public StringFormatMethodAttribute(string formatParameterName)
      {
         FormatParameterName = formatParameterName;
      }
   }

}
// ReSharper restore InconsistentNaming
