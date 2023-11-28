using Assets.Scripts.Models.Characters;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Users
{
    /// <inheritdoc/>
    public sealed class UserData : IUserData
    {
        /// <inheritdoc/>
        public int Money { get; private set; }

        /// <inheritdoc/>
        public int BestMetersRecord { get; private set; }

        /// <inheritdoc/>
        public List<string> CharacterKeys { get; private set; } = new () { "fb7df1cf4762c4f98935c2b2e6bb8fb3" };

        /// <inheritdoc/>
        public void EarnMoney(int money)
        {
            if (money < 0)
            {
                return;
            }

            Money += money;
            Commit();
        }

        /// <inheritdoc/>
        public bool BuyCharacter(ICharacterModel character)
        {
            if (CharacterKeys.Contains(character.CharacterKey))
            {
                return false;
            }

            if (Money < character.CharacterPrice)
            {
                return false;
            }

            Money -= character.CharacterPrice;
            CharacterKeys.Add(character.CharacterKey);
            Commit();

            return true;
        }

        /// <inheritdoc/>
        public bool TrySetBestMetersRecord(int meters)
        {
            if (BestMetersRecord < meters)
            {
                BestMetersRecord = meters;
                Commit();

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Commit()
        {

        }

        /// <inheritdoc/>
        public void Fetch()
        {

        }

        public void Reset()
        {
            Money = 0;
            BestMetersRecord = 0;
            CharacterKeys = new() { "fb7df1cf4762c4f98935c2b2e6bb8fb3" };

            Commit();
        }
    }
}
