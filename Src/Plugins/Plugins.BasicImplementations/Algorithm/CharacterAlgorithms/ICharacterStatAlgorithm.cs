using ChickenAPI.Enums.Game.Character;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms
{
    public interface ICharacterStatAlgorithm
    {
        void Initialize();

        int GetStat(CharacterClassType type, byte level);
    }
}