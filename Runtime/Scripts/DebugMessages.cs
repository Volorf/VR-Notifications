using System.Collections.Generic;
using UnityEngine;

namespace Volorf.VRNotifications
{
    [CreateAssetMenu(fileName = "Debug Messages", menuName = "Create Debug Messages")]
    public class DebugMessages : ScriptableObject
    {
        public List<string> Messages;
    }
}
