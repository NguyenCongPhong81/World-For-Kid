using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace Script
{
    public struct AuthenRPCData : INetworkSerializable
    {
        public FixedString128Bytes UserName;
        public FixedString128Bytes DisplayName;
        public bool IsRedTem;
        public int IndexCharacter;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UserName);
            serializer.SerializeValue(ref DisplayName);
            serializer.SerializeValue(ref IsRedTem);
            serializer.SerializeValue(ref IndexCharacter);
        }
    }
    
    public struct MapClientIdWithUserName : INetworkSerializable
    {
        public ulong ClientId;
        public FixedString128Bytes UserName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UserName);
            serializer.SerializeValue(ref ClientId);
        }
    }

    public class PlayerInGameData
    {
        public string UserName;
        public string DisplayName;
        public bool IsRedTem;
        public int IndexPosition = -1;
        public int CharacterType;
        public int CurrentQuestion;
        public PlayerState State;
        public int LastAnswerIdSelected;
        public int Heath;
        public int Energy;
        public bool HaveShield;
        public int BonusAttackNormal;
        public int TimeEffectMinusIndexQuestion;

        public PlayerInGameRPCData ConvertToRPC()
        {
            return new PlayerInGameRPCData
            {
                UserName = UserName,
                DisplayName = DisplayName,
                IsRedTem = IsRedTem,
                IndexPosition = IndexPosition,
                CharacterType = CharacterType,
                CurrentQuestion = CurrentQuestion,
                State = State,
                LastAnswerIdSelected = LastAnswerIdSelected,
                Heath = Heath,
                Energy = Energy,
                HaveShield = HaveShield,
                BonusAttackNormal = BonusAttackNormal,
                TimeEffectMinusIndexQuestion = TimeEffectMinusIndexQuestion,
            };
        }
    }

    public struct PlayerInGameRPCData : INetworkSerializable
    {
        public FixedString128Bytes UserName;
        public FixedString128Bytes DisplayName;
        public bool IsRedTem;
        public int CharacterType;
        public int CurrentQuestion;
        public int IndexPosition;
        public PlayerState State;
        public int LastAnswerIdSelected;
        public int Heath;
        public int Energy;
        public bool HaveShield;
        public int BonusAttackNormal;
        public int TimeEffectMinusIndexQuestion;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UserName);
            serializer.SerializeValue(ref DisplayName);
            serializer.SerializeValue(ref IsRedTem);
            serializer.SerializeValue(ref CharacterType);
            serializer.SerializeValue(ref CurrentQuestion);
            serializer.SerializeValue(ref IndexPosition);
            serializer.SerializeValue(ref State);
            serializer.SerializeValue(ref LastAnswerIdSelected);
            serializer.SerializeValue(ref Heath);
            serializer.SerializeValue(ref Energy);
            serializer.SerializeValue(ref HaveShield);
            serializer.SerializeValue(ref BonusAttackNormal);
            serializer.SerializeValue(ref TimeEffectMinusIndexQuestion);
        }

        public PlayerInGameData ConvertToNormal()
        {
            return new PlayerInGameData
            {
                UserName = UserName.ToString(),
                DisplayName = DisplayName.ToString(),
                IsRedTem = IsRedTem,
                CharacterType = CharacterType,
                CurrentQuestion = CurrentQuestion,
                IndexPosition = IndexPosition,
                State = State,
                LastAnswerIdSelected = LastAnswerIdSelected,
                Heath = Heath,
                Energy = Energy,
                HaveShield = HaveShield,
                BonusAttackNormal = BonusAttackNormal,
                TimeEffectMinusIndexQuestion = TimeEffectMinusIndexQuestion
            };
        }
    }

    public class QuestionData
    {
        public int Id;
        public string Text;
        public List<AnswerData> AnswerDatas;
        public int CorrectAnswerId;

        public QuestionDataRPC ConvertToRpc()
        {
            var questionDataRPC = new QuestionDataRPC
            {
                Id = this.Id,
                Text = this.Text,
                AnswerDatas = new AnswerDataRPC[this.AnswerDatas.Count],
                CorrectId = CorrectAnswerId
            };


            for (int i = 0; i < this.AnswerDatas.Count; ++i)
            {
                questionDataRPC.AnswerDatas[i] = this.AnswerDatas[i].ConvertRpc();
            }

            return questionDataRPC;
        }
    }

    public class AnswerData
    {
        public int Id;
        public string Text;

        public AnswerDataRPC ConvertRpc()
        {
            return new AnswerDataRPC
            {
                Id = this.Id,
                Text = this.Text
            };
        }
    }

    public struct QuestionDataRPC : INetworkSerializable
    {
        public int Id;
        public FixedString4096Bytes Text;
        public AnswerDataRPC[] AnswerDatas;
        public int CorrectId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Text);
            serializer.SerializeValue(ref AnswerDatas);
            serializer.SerializeValue(ref CorrectId);
        }

        public QuestionData ConvertToNormal()
        {
            var questionData = new QuestionData
            {
                Id = this.Id,
                Text = this.Text.ToString(),
                AnswerDatas = new List<AnswerData>(),
                CorrectAnswerId = CorrectId
            };


            for (int i = 0; i < this.AnswerDatas.Length; ++i)
            {
                questionData.AnswerDatas.Add(this.AnswerDatas[i].ConvertToNormal());
            }

            return questionData;
        }
    }

    public struct AnswerDataRPC : INetworkSerializable
    {
        public int Id;
        public FixedString128Bytes Text;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Text);
        }

        public AnswerData ConvertToNormal()
        {
            return new AnswerData
            {
                Id = Id,
                Text = Text.ToString()
            };
        }
    }
}