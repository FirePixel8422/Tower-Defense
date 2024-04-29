using UnityEngine;



[CreateAssetMenu(fileName = "New Wave", menuName = "WaveSystem")]
public class WaveDataSO : ScriptableObject
{
    public WavePart[] waveParts;

    [System.Serializable]
    public class WavePart
    {
        public float startDelay;
        public EnemyCore enemy;

        [Header("Delay between spawned creatures")]
        public float spawnDelay;
        public int amount;
    }
    public float waveEndDelay;
}

public enum Element
{
    Neutral,
    Life,
    Arcane,
    Ember
}
