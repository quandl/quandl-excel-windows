# Coded UI Tests

1. Set the `AutomationProperties.AutomatedID` on properties that should be tested against before making a recording.
    - In cases where it represents a single item that you are selecting, the `AutomationID` should reflect the text
    value which is visible to the user
    - When placing an `AutomationID` on an element which serves as a wrapper for items (ie. ListView and ListItems), the
    wrapper should be a name representing the parent holder.
- Create a new **Coded UI Test** file
    - Click **Cancel** when prompted to use the recorder
    - Refactor the following code:
        - TestContext property needs to be refactored to use the UIMap file
        - Uncomment the test initialization and cleanup methods in the "Additional test attributes"
- Record actions & edit the default actions before saving recording (ie. remove _Select 'book1' client_ from actions).
    - Can be removed from the "Show Recorded Steps" menu prior to saving
    - Can also be removed from the `UIMap.uitest` file after saving the test.
- Extract tests from the `UIMap.designer.cs` file into the `UIMap.cs` file.
- Update the `Settings.xml` file in the Quandl.Test.CodedUI project to include the QA `username`, `password` and
`api_key`

## Additional Resources:

1. The following articles are helpful for learning how to test against a collection of child elements residing within a
common parent element.  One example where this can be useful is for running assertions on the total number of children.
  - https://blogs.msdn.microsoft.com/thejs_blog/2014/09/04/findmatchingcontrols-for-wpf-windows/
  - http://executeautomation.com/blog/how-to-identify-all-the-child-elements-within-a-page-in-coded-ui-test-cuit/

## Known Bugs:
- Cannot find virtualized attribute (first or all playback attempts). Solution: create your own virtualized elements
to correct this issue
