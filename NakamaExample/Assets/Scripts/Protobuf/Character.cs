// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: character.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace NakamaMinimalGame.Character {

  /// <summary>Holder for reflection information generated from character.proto</summary>
  public static partial class CharacterReflection {

    #region Descriptor
    /// <summary>File descriptor for character.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CharacterReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9jaGFyYWN0ZXIucHJvdG8SBG1haW4ioQEKCUNoYXJhY3RlchIRCgljbGFz",
            "c25hbWUYASABKAkSDQoFbGV2ZWwYAiABKAUSIQoZZXF1aXBwZWRfaXRlbV9t",
            "YWluaGFuZF9pZBgEIAEoAxIgChhlcXVpcHBlZF9pdGVtX29mZmhhbmRfaWQY",
            "BSABKAMSFgoOY3VycmVudF9oZWFsdGgYBiABKAISFQoNY3VycmVudF9wb3dl",
            "chgHIAEoAkIeqgIbTmFrYW1hTWluaW1hbEdhbWUuQ2hhcmFjdGVyYgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::NakamaMinimalGame.Character.Character), global::NakamaMinimalGame.Character.Character.Parser, new[]{ "Classname", "Level", "EquippedItemMainhandId", "EquippedItemOffhandId", "CurrentHealth", "CurrentPower" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Character : pb::IMessage<Character> {
    private static readonly pb::MessageParser<Character> _parser = new pb::MessageParser<Character>(() => new Character());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Character> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::NakamaMinimalGame.Character.CharacterReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Character() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Character(Character other) : this() {
      classname_ = other.classname_;
      level_ = other.level_;
      equippedItemMainhandId_ = other.equippedItemMainhandId_;
      equippedItemOffhandId_ = other.equippedItemOffhandId_;
      currentHealth_ = other.currentHealth_;
      currentPower_ = other.currentPower_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Character Clone() {
      return new Character(this);
    }

    /// <summary>Field number for the "classname" field.</summary>
    public const int ClassnameFieldNumber = 1;
    private string classname_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Classname {
      get { return classname_; }
      set {
        classname_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "level" field.</summary>
    public const int LevelFieldNumber = 2;
    private int level_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Level {
      get { return level_; }
      set {
        level_ = value;
      }
    }

    /// <summary>Field number for the "equipped_item_mainhand_id" field.</summary>
    public const int EquippedItemMainhandIdFieldNumber = 4;
    private long equippedItemMainhandId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long EquippedItemMainhandId {
      get { return equippedItemMainhandId_; }
      set {
        equippedItemMainhandId_ = value;
      }
    }

    /// <summary>Field number for the "equipped_item_offhand_id" field.</summary>
    public const int EquippedItemOffhandIdFieldNumber = 5;
    private long equippedItemOffhandId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long EquippedItemOffhandId {
      get { return equippedItemOffhandId_; }
      set {
        equippedItemOffhandId_ = value;
      }
    }

    /// <summary>Field number for the "current_health" field.</summary>
    public const int CurrentHealthFieldNumber = 6;
    private float currentHealth_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float CurrentHealth {
      get { return currentHealth_; }
      set {
        currentHealth_ = value;
      }
    }

    /// <summary>Field number for the "current_power" field.</summary>
    public const int CurrentPowerFieldNumber = 7;
    private float currentPower_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float CurrentPower {
      get { return currentPower_; }
      set {
        currentPower_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Character);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Character other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Classname != other.Classname) return false;
      if (Level != other.Level) return false;
      if (EquippedItemMainhandId != other.EquippedItemMainhandId) return false;
      if (EquippedItemOffhandId != other.EquippedItemOffhandId) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(CurrentHealth, other.CurrentHealth)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(CurrentPower, other.CurrentPower)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Classname.Length != 0) hash ^= Classname.GetHashCode();
      if (Level != 0) hash ^= Level.GetHashCode();
      if (EquippedItemMainhandId != 0L) hash ^= EquippedItemMainhandId.GetHashCode();
      if (EquippedItemOffhandId != 0L) hash ^= EquippedItemOffhandId.GetHashCode();
      if (CurrentHealth != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(CurrentHealth);
      if (CurrentPower != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(CurrentPower);
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Classname.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Classname);
      }
      if (Level != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Level);
      }
      if (EquippedItemMainhandId != 0L) {
        output.WriteRawTag(32);
        output.WriteInt64(EquippedItemMainhandId);
      }
      if (EquippedItemOffhandId != 0L) {
        output.WriteRawTag(40);
        output.WriteInt64(EquippedItemOffhandId);
      }
      if (CurrentHealth != 0F) {
        output.WriteRawTag(53);
        output.WriteFloat(CurrentHealth);
      }
      if (CurrentPower != 0F) {
        output.WriteRawTag(61);
        output.WriteFloat(CurrentPower);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Classname.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Classname);
      }
      if (Level != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Level);
      }
      if (EquippedItemMainhandId != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(EquippedItemMainhandId);
      }
      if (EquippedItemOffhandId != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(EquippedItemOffhandId);
      }
      if (CurrentHealth != 0F) {
        size += 1 + 4;
      }
      if (CurrentPower != 0F) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Character other) {
      if (other == null) {
        return;
      }
      if (other.Classname.Length != 0) {
        Classname = other.Classname;
      }
      if (other.Level != 0) {
        Level = other.Level;
      }
      if (other.EquippedItemMainhandId != 0L) {
        EquippedItemMainhandId = other.EquippedItemMainhandId;
      }
      if (other.EquippedItemOffhandId != 0L) {
        EquippedItemOffhandId = other.EquippedItemOffhandId;
      }
      if (other.CurrentHealth != 0F) {
        CurrentHealth = other.CurrentHealth;
      }
      if (other.CurrentPower != 0F) {
        CurrentPower = other.CurrentPower;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Classname = input.ReadString();
            break;
          }
          case 16: {
            Level = input.ReadInt32();
            break;
          }
          case 32: {
            EquippedItemMainhandId = input.ReadInt64();
            break;
          }
          case 40: {
            EquippedItemOffhandId = input.ReadInt64();
            break;
          }
          case 53: {
            CurrentHealth = input.ReadFloat();
            break;
          }
          case 61: {
            CurrentPower = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
