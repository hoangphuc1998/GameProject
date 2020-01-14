using System;
[Serializable]
public class PKM
{
    public string id;
    public int hp;
    public int attack;
    public int def;
    public int power1;
    public int power2;
    public void copyFrom(PKM other) {
        id = other.id;
        hp = other.hp;
        attack = other.attack;
        def = other.def;
        power1 = other.power1;
        power2 = other.power2;
    }
}
