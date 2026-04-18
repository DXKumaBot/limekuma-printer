using System.ComponentModel;

namespace Limekuma.Prober.Lxns.Enums;

[DefaultValue(Dummy)]
public enum Difficulty
{
    Dummy = -1,
    Basic,
    Advanced,
    Expert,
    Master,
    ReMaster
}
