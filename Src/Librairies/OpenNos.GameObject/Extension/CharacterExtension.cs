using System;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace OpenNos.GameObject.Extension
{
    public static class CharacterExtension
    {
        #region Methods

        //public static void SendSomePacket(this Character e)
        //{
        //    var session = e.Session;
        //    session.SendPacket("rsfi 7 1 10 10 10 10");
        //    session.SendPacket("sqst 0 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //    session.SendPacket("sqst 1 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //    session.SendPacket("sqst 2 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //    session.SendPacket("sqst 3 00000000000000000000000000000000000000000000000000000000000000000000000000000000000U}}t}}V}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}00U}}000000UX00000000000000000000000000000000000000000000000000000000000000000000000zW000000000000000");
        //    session.SendPacket("sqst 4 00UX00O}Z0009000000000UW000000000000000000000000000000UW000000000000000000000000000U}}z}}}}}}}}}}z}zV}}}l000000000000OWW0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //    session.SendPacket("sqst 5 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //    session.SendPacket("sqst 6 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        //}

        public static void GetBuffFromSet(this Character e)
        {
            // (MainWeapon, SecondaryWeapon, Armor, Card) Vnum
            var set = e.Class == CharacterClassType.Archer ? new Tuple<long, long, long, short>(4966, 4963, 4954, 45) :
                e.Class == CharacterClassType.Magician ? new Tuple<long, long, long, short>(4965, 4962, 4953, 45) :
                e.Class == CharacterClassType.Swordsman ? new Tuple<long, long, long, short>(4964, 4961, 4952, 45) :
                e.Class == CharacterClassType.MartialArtist ? new Tuple<long, long, long, short>(4736, 4767, 4754, 45) :
                //e.Class == ClassType.Adventurer ? new Tuple<long, long, long, short>(566, 3636, 1313, 1) :
                new Tuple<long, long, long, short>(566, 3636, 1313, 1);

            e.GetBuffFromSet(set);

            set = e.Class == CharacterClassType.Archer ? new Tuple<long, long, long, short>(4960, 4957, 4951, 46) :
                e.Class == CharacterClassType.Magician ? new Tuple<long, long, long, short>(4959, 4956, 4950, 46) :
                e.Class == CharacterClassType.Swordsman ? new Tuple<long, long, long, short>(4958, 4955, 4949, 46) :
                e.Class == CharacterClassType.MartialArtist ? new Tuple<long, long, long, short>(4736, 4767, 4754, 46) :
                //e.Class == ClassType.Adventurer ? new Tuple<long, long, long, short>(566, 3636, 1313, 1) :
                new Tuple<long, long, long, short>(566, 3636, 1313, 1);

            e.GetBuffFromSet(set);
        }

        public static void GetBuffFromSet(this Character e, Tuple<long, long, long, short> set)
        {
            if (!e.HaveThisStuffWeared(EquipmentType.MainWeapon, set.Item1) ||
                !e.HaveThisStuffWeared(EquipmentType.SecondaryWeapon, set.Item2) ||
                !e.HaveThisStuffWeared(EquipmentType.Armor, set.Item3))
            {
                return;
            }

            if (e.Buff.ContainsKey(set.Item4))
            {
                return;
            }

            e.RemoveSetBuff();
            e.AddBuff(new Buff(set.Item4, 1, true), e.BattleEntity);
        }

        public static void GoldLess(this ClientSession session, long value)
        {
            session.Character.Gold -= value;
            if (session.Character.Gold <= 0) session.Character.Gold = 0;

            session.SendPacket(session.Character.GenerateGold());
        }

        public static void GoldUp(this ClientSession session, long value)
        {
            session.Character.Gold += value;
            session.SendPacket(session.Character.GenerateGold());
        }

        public static bool HaveThisStuffWeared(this Character e, EquipmentType type, long Vnum)
        {
            var item = e.Inventory.LoadBySlotAndType((byte) type, InventoryType.Wear);
            if (item == null)
            {
                return false;
            }

            if (item.ItemVNum != Vnum)
            {
                return false;
            }

            return true;
        }

        public static void RemoveSetBuff(this Character e)
        {
            e.RemoveBuff(45, true);
            e.RemoveBuff(46, true);
        }

        public static string GetFamilyNameType(this Character e)
        {
            var thisRank = e.FamilyCharacter.Authority;

            return thisRank == FamilyAuthority.Member ? "918" :
                thisRank == FamilyAuthority.Familydeputy ? "916" :
                thisRank == FamilyAuthority.Familykeeper ? "917" :
                thisRank == FamilyAuthority.Head ? "915" : "-1 -";
        }
        public static string GetClassType(this Character e)
        {
            var thisClass = e.Class;

            return thisClass == CharacterClassType.Adventurer ? "35" :
                thisClass == CharacterClassType.Swordsman ? "36" :
                thisClass == CharacterClassType.Archer ? "37" :
                thisClass == CharacterClassType.Magician ? "38" :
                thisClass == CharacterClassType.MartialArtist ? "39" : "0";


        }
        public static void SendShopEnd(this ClientSession s)
        {
            s.SendPacket("shop_end 2");
            s.SendPacket("shop_end 1");
        }

        #endregion
    }
}