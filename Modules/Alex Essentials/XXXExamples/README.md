# Attributes Class

*The Attributes class is a set of tools aimed at giving developers easy customization and control over fields in the
inspector.*

> Authored, developed and updated by [**@ltsLumina**] on GitHub (https://github.com/ltsLumina)
> > All credit goes to [**@heisarzola**] on GitHub (https://github.com/heisarzola)
> for creating the RangedFloat attribute and the drawers. I simply added them to my essentials package for easy access.

**Attributes Class:**
![attribute example.gif](..%2F..%2FEditor%2FImgs%2Fattribute%20example.gif)

## General Notes

- ReadOnly Attribute is used to make a field visible but not editable in the inspector.
  - Usage: [ReadOnly] or [SerializeField, ReadOnly]
- RangedFloat allows developers to assign a range between minimum and maximum float values.
  - Usage: [RangedFloat(min, max, RangeDisplayType)]
- There are three types of display for the range: LockedRanges, EditableRanges, and HideRanges.
  - LockedRanges: range is not editable.
  - EditableRanges: user can edit the range.
  - HideRanges: range is not visible hence locked.
- The GetRandomValue() method fetches a random value between the assigned min and max float values.

## Example:

```csharp
    [Header("ReadOnly Attribute")]
    [SerializeField, ReadOnly] private int readOnlyInt;

    [Header("Ranged Float Without Attribute")]
    [SerializeField] RangedFloat exampleOne;

    [Header("Ranged Float With Locked Limits")]
    [RangedFloat(5, 25, RangedFloatAttribute.RangeDisplayType.LockedRanges)]
    [SerializeField] RangedFloat exampleTwo;

    [Header("Ranged Float With Editable Limits")]
    [RangedFloat(0, 10, RangedFloatAttribute.RangeDisplayType.EditableRanges)]
    [SerializeField] RangedFloat exampleThree;

    [Header("Ranged Float With Hidden (But Locked) Limits")]
    [RangedFloat(5, 10, RangedFloatAttribute.RangeDisplayType.HideRanges)]
    [SerializeField] RangedFloat exampleFour;
```

If any doubt on how to use the attributes arises, please see the provided examples for reference. (Examples.cs)

# Sequencer
*The Sequencer is a tool that allows you to create a sequence of events that can be triggered by a single call.*

> Designed, created and maintained by [**@ltsLumina**] on GitHub (https://github.com/ltsLumina)

**End Result:**
![sequencing example.gif](..%2F..%2FEditor%2FImgs%2Fsequencing%20example.gif)

## General Notes

- The Sequencer is a tool that allows you to create a sequence of events that can be triggered by a single call.
- Don't forget to make use of anonymous methods and lambda expressions to make your code more readable.
- All methods are executed in the same order they are called.
  - i.e. Execute(First).Execute(Second).Execute(Third) will execute First, then Second, then Third.
  - i.e. Execute(First).WaitForSeconds(2f).Execute(Second) will execute First, then wait 2 seconds, then execute Second.
- Take a look at the provided examples script for reference. (Examples.cs)

## Example:

```csharp
        // Example 1 -- This is the recommended way to use Sequencing.
        Sequence sequenceOne = Sequencing.CreateSequence(this); // Creates a sequence.
        sequenceOne.Execute(FirstExample).WaitForSeconds(2f).Execute(SecondExample); 


        // Example 2 -- A longer example of how to create longer sequences.
        Sequence sequenceTwo = Sequencing.CreateSequence(this);
        sequenceTwo.WaitThenExecute(5f, ExampleMethod3).ContinueWith(() => Debug.Log("Finished!")).WaitForSeconds(2f).Execute(FirstExample);


        // Example 3 -- An example of how you can make a more readable sequence.
        Sequencing.CreateSequence(this)
                  .Execute(() => Debug.Log("Hello World!"))
                  .WaitForSeconds(3f)
                  .ContinueWith(() => Debug.Log("Goodbye World!"));
```

