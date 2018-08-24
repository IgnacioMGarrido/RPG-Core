using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {
    private enum OperationAction {
        Addition,
        Substraction
    }

    // Attributes
    [SerializeField] int might = 10;
    [SerializeField] int constitution = 10;
    [SerializeField] int dexterity = 10;
    [SerializeField] int perception = 10;
    [SerializeField] int intellect = 10;
    [SerializeField] int resolve = 10;


    //Defenses
    [SerializeField] float fortitude;
    [SerializeField] float reflex;
    [SerializeField] float deflection;
    [SerializeField] float will;

    //Passive Stats
    [SerializeField] float classHealthMutiplier = 5;
    [SerializeField] float endurance = 20;
    [SerializeField] float health = 0;
    [SerializeField] float concentration = 33;

    //Action Stats
    [SerializeField] float damage = 10;
    [SerializeField] float healing = 7;
    [SerializeField] float actionSpeed = 3;
    [SerializeField] float interrupt = 0;
    [SerializeField] float accuarcy = 0;
    [SerializeField] float areaOfEffect = 7;
    [SerializeField] float duration = 3;

    public int Might
    {
        get
        {
            return might;
        }

        set
        {
            might = value;
        }
    }

    public int Constitution
    {
        get
        {
            return constitution;
        }

        set
        {
            constitution = value;
        }
    }

    public int Dexterity
    {
        get
        {
            return dexterity;
        }

        set
        {
            dexterity = value;
        }
    }

    public int Perception
    {
        get
        {
            return perception;
        }

        set
        {
            perception = value;
        }
    }

    public int Intellect
    {
        get
        {
            return intellect;
        }

        set
        {
            intellect = value;
        }
    }

    public int Resolve
    {
        get
        {
            return resolve;
        }

        set
        {
            resolve = value;
        }
    }

    public float Fortitude
    {
        get
        {
            return fortitude;
        }

        set
        {
            fortitude = value;
        }
    }

    public float Reflex
    {
        get
        {
            return reflex;
        }

        set
        {
            reflex = value;
        }
    }

    public float Deflection
    {
        get
        {
            return deflection;
        }

        set
        {
            deflection = value;
        }
    }

    public float Will
    {
        get
        {
            return will;
        }

        set
        {
            will = value;
        }
    }

    public float ClassHealthMutiplier
    {
        get
        {
            return classHealthMutiplier;
        }

        set
        {
            classHealthMutiplier = value;
        }
    }

    public float Endurance
    {
        get
        {
            return endurance;
        }

        set
        {
            endurance = value;
        }
    }

    public float Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
        }
    }

    public float Concentration
    {
        get
        {
            return concentration;
        }

        set
        {
            concentration = value;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    public float Healing
    {
        get
        {
            return healing;
        }

        set
        {
            healing = value;
        }
    }

    public float ActionSpeed
    {
        get
        {
            return actionSpeed;
        }

        set
        {
            actionSpeed = value;
        }
    }

    public float Interrupt
    {
        get
        {
            return interrupt;
        }

        set
        {
            interrupt = value;
        }
    }

    public float Accuarcy
    {
        get
        {
            return accuarcy;
        }

        set
        {
            accuarcy = value;
        }
    }

    public float AreaOfEffect
    {
        get
        {
            return areaOfEffect;
        }

        set
        {
            areaOfEffect = value;
        }
    }

    public float Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }

    private void Awake()
    {
        //TODO: fix hardcoded percentages.
        //Defenses
        fortitude = might * 2 + constitution * 2;
        reflex = dexterity * 2 + perception * 2;
        deflection = resolve * 1;
        will = intellect * 2 + resolve * 2;

        //Passive Stats
        health = classHealthMutiplier * endurance;
        endurance = CalculatePercentage(endurance, constitution, 0.05f, OperationAction.Addition);
        health = CalculatePercentage(health, constitution, 0.05f, OperationAction.Addition);
        concentration = resolve * 3;

        //Action Stats
        damage = CalculatePercentage(damage, might, 0.03f, OperationAction.Addition);
        healing = CalculatePercentage(healing, might, 0.03f, OperationAction.Addition);
        actionSpeed = CalculatePercentage(actionSpeed, dexterity, 0.06f, OperationAction.Substraction);
        interrupt = perception * 3;
        accuarcy = perception * 1;
        areaOfEffect = CalculatePercentage(areaOfEffect, intellect, 0.06f, OperationAction.Addition);
        duration = CalculatePercentage(duration, intellect, 0.05f, OperationAction.Addition);


    }

    float CalculatePercentage(float stateToCalculate, int attribute, float percentage, OperationAction _oa) {

        //if it is less than the neutral point then we do the opposite operation
        if (attribute < 10) {
            if (_oa == OperationAction.Addition)
                _oa = OperationAction.Substraction;
            else
                _oa = OperationAction.Substraction;
        }

        switch (_oa)
        {
            case OperationAction.Addition:
                for (int i = 0; i < attribute; i++)
                {
                    stateToCalculate += stateToCalculate * percentage;
                }
                break;
            case OperationAction.Substraction:
                for (int i = 0; i < attribute; i++)
                {
                    stateToCalculate -= stateToCalculate * percentage;
                }
                stateToCalculate = Mathf.Clamp(stateToCalculate, 0.5f, stateToCalculate);
                break;
        }


        return stateToCalculate;

    }


}
