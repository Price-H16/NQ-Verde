using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Magical
{
    public class MagicDodgeAlgorithm : ICharacterStatAlgorithm
    {
        public void Initialize()
        {
            // no dodge possible without shells effects
        }

        public int GetStat(CharacterClassType type, byte level) => 0;
    }
}