using UnityEngine;

namespace Core.DaysCycle
{
    [CreateAssetMenu(fileName = "DayDataSO", menuName = "DayCycle/DayData")]
    public sealed class DayDataSO : ScriptableObject
    {
        [SerializeField] private int _fishCountToEndDay = 4;

        public int FishCountToEndDay => _fishCountToEndDay;
    }
}