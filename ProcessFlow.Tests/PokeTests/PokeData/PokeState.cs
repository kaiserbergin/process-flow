using System.Collections.Generic;

namespace ProcessFlow.Tests.PokeTests.PokeData
{
    public class PokeState
    {
        public List<Pokemon> MyPokemon { get; set; } = new List<Pokemon>();
        public int PokeBallCount { get; set; } = 6;
        public Pokemon EncounteredMon { get; set; }
        public int DesiredPokemon { get; set; } = 6;
    }

    public class Pokemon
    {
        public PokeSpecies PokeSpecies { get; set; }
        public string NickName { get; set; }
        public int Level { get; set; }
        public double BaseCaptureChance { get; set; }
        public void StateName() { }
    }

    public enum PokeSpecies
    {
        Charmander,
        Litten,
        Squirtle,
        Sobble,
        Bulbasaur,
        Chikorita,
        Pikachu,
        Pichu
    }
}