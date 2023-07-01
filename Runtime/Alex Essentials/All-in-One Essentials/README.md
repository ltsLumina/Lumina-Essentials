# Ranged Float Attribute
*The RangedFloat attribute, drawers and class collection is a set of tools intended to easily draw in inspector a range
between two float values.*

> All credit goes to [**@heisarzola**] on GitHub (https://github.com/heisarzola) 
> for creating this attribute and the drawers. I simply added them to my essentials package for easy access.

**End Result:**

![Ranged Float](https://github.com/ltsLumina/Unity-Essentials/assets/119983088/4b8f4090-a486-4821-b75d-a5c4fb15f010)


If any doubt on how to use the attribute arises, please see the provided examples for reference.

## General Notes

- The RangedFloatAttribute (what is between brackets) will only work on the provided RangedFloat class.
- The RangedFloat comes with a built in random within range method. If you use a RangedFloat instance as a float, it will
automatically fetch a random value between min and max.
  - i.e. myFloat = myRangedFloat; // myFloat now has a value between myRangedFloat's min and max.
  - Likewise, you can call it explicitly if you want to: i.e. myFloat = myRangedFloat.GetRandomValue();// Same result as
above.
- You can download this folder only instead of the whole project if you want to.

## Example:
>

    [Header("Ranged Float Without Attribute")]
    [SerializeField] RangedFloat exampleOne;

    [Header("Ranged Float With Locked Limits")]
    [RangedFloat(2, 25, RangedFloatAttribute.RangeDisplayType.LockedRanges)]
    [SerializeField] RangedFloat exampleTwo;

    [Header("Ranged Float With Editable Limits")]
    [RangedFloat(0, 1, RangedFloatAttribute.RangeDisplayType.EditableRanges)]
    [SerializeField] RangedFloat exampleThree;

    [Header("Ranged Float With Hidden (But Locked) Limits")]
    [RangedFloat(5, 10, RangedFloatAttribute.RangeDisplayType.HideRanges)]
    [SerializeField] RangedFloat exampleFour;
    
Try using any of these code snippets to see what works best for you.
