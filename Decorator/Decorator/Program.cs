using System;

public interface IHero
{
    string GetDescription();
    int GetAttack();
    int GetDefense();
}

public class Warrior : IHero
{
    public string GetDescription() => "Warrior";
    public int GetAttack() => 10;
    public int GetDefense() => 5;
}

public class Mage : IHero
{
    public string GetDescription() => "Mage";
    public int GetAttack() => 15;
    public int GetDefense() => 3;
}

public class Paladin : IHero
{
    public string GetDescription() => "Paladin";
    public int GetAttack() => 8;
    public int GetDefense() => 7;
}

public abstract class InventoryDecorator : IHero
{
    protected IHero hero;

    public InventoryDecorator(IHero hero)
    {
        this.hero = hero;
    }

    public virtual string GetDescription() => hero.GetDescription();
    public virtual int GetAttack() => hero.GetAttack();
    public virtual int GetDefense() => hero.GetDefense();
}

public class Sword : InventoryDecorator
{
    public Sword(IHero hero) : base(hero) { }

    public override string GetDescription() => $"{hero.GetDescription()} with Sword";
    public override int GetAttack() => hero.GetAttack() + 5;
}

public class Armor : InventoryDecorator
{
    public Armor(IHero hero) : base(hero) { }

    public override string GetDescription() => $"{hero.GetDescription()} with Armor";
    public override int GetDefense() => hero.GetDefense() + 4;
}

public class MagicAmulet : InventoryDecorator
{
    public MagicAmulet(IHero hero) : base(hero) { }

    public override string GetDescription() => $"{hero.GetDescription()} with Magic Amulet";
    public override int GetAttack() => hero.GetAttack() + 3;
    public override int GetDefense() => hero.GetDefense() + 2;
}

class Program
{
    static void Main(string[] args)
    {
        IHero warrior = new Warrior();
        Console.WriteLine($"{warrior.GetDescription()}, Attack: {warrior.GetAttack()}, Defense: {warrior.GetDefense()}\n");

        warrior = new Sword(warrior);
        warrior = new Armor(warrior);
        Console.WriteLine($"{warrior.GetDescription()}, Attack: {warrior.GetAttack()}, Defense: {warrior.GetDefense()}\n");

        IHero mage = new Mage();
        mage = new MagicAmulet(mage);
        mage = new Sword(mage);
        Console.WriteLine($"{mage.GetDescription()}, Attack: {mage.GetAttack()}, Defense: {mage.GetDefense()}\n");

        IHero paladin = new Paladin();
        paladin = new Sword(paladin);
        paladin = new Armor(paladin);
        paladin = new MagicAmulet(paladin);
        Console.WriteLine($"{paladin.GetDescription()}, Attack:  {paladin.GetAttack()}, Defense:  {paladin.GetDefense()} \n");
    }
}