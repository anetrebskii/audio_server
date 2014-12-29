using System;
using System.Configuration;

namespace Alnet.AudioServer.Components.NAudioServer
{
   public sealed class AudioServerConfiguration : ConfigurationSection
   {
      [ConfigurationProperty("channels", IsRequired = false)]
      public ChannelsElementCollection Channels
      {
         get { return (ChannelsElementCollection)this["channels"]; }
         set { this["channels"] = value; }
      }

      public ConfigurationPropertyCollection Collection { get; set; }
   }

   public sealed class ChannelsElementCollection : ConfigurationElementCollection
   {
      protected override ConfigurationElement CreateNewElement()
      {
         return new ChannelElement();
      }

      public override ConfigurationElementCollectionType CollectionType
      {
         get
         {
            return ConfigurationElementCollectionType.BasicMap;
         }
      }

      protected override string ElementName
      {
         get { return "channel"; }
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
         var elm = element as ChannelElement;
         if (elm == null)
         {
            throw new ArgumentNullException();
         }
         return elm.Index;
      }
   }

   public sealed class ChannelElement : ConfigurationElement
   {
      [ConfigurationProperty("index", IsRequired = true)]
      public int Index
      {
         get { return (int)this["index"]; }
         set { this["index"] = value; }
      }

      [ConfigurationProperty("name", IsRequired = true)]
      public string Name
      {
         get { return this["name"].ToString(); }
         set { this["name"] = value; }
      }
   }
}
