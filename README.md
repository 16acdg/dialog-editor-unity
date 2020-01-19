# dialog-editor-unity
Custom dialog GUI editor for the Unity game engine.
Visually displays conversation as a series of nodes in a graph.

![alt text](https://andrewgray.me/images/dialogeditor.png)

Feel free to use this code in any of your projects. I will probably try to put it on the Unity Asset Store in the near future.


## Usage
To implement the dialog manager into your unity project, there are two options:

1) Insert the dialog manager unity package from this repo, and check out the sample scene and the sample prefabs.

2) Insert just the scripts and create GameObjects as you see fit.

The sample project, and any project incorporating this code, will have these three components:
• __DialogManager__ - where you attach DialogManager.cs and then manually set any dialog UI references.
• __Dialog UI Container__ - holds all UI components for the dialog.
• __NPC(s)__ - Some script attached to an NPC with which the player can interact should implement the IConversable interface. This just acts as a tag. 
• __Conversations__ - Each conversation should be implemented as distinct game objects. A conversation game object will only have a Transform and a Conversation.cs instance.

To get NPCs to initiate conversations with the player, the __StartConversation__ method of a conversation instance needs to be invoked.
To actually edit the dialog pertaining to a Conversation instance, you first need to set the path where the json files holding conversation data will be saved. This setting is done in the DialogManager game object. Make sure to hit the __Set Dialog Save Path__ button after setting this save path. Next, select a conversation game object in the project hierarchy and click the __Edit Dialog__ button in the inspector. This will launch the Dialog Editor window within Unity for the conversation instance you selected. 

### Using the Dialog Editor Window
In the Dialog Editor window, you will notice a dialog __Start__ and __End__ node. Clicking on either node will show text fields at the bottom of the editor where you can edit the conversation. The text field in the bottom left corner is the dialog that the _NPC will speak to the player_. To add more nodes to the conversation, press the __+__ button in the upper right corner.  To move a node, just click and drag. To create new branches between the nodes, right-click on a node, then move the cursor to the other node you want to connect to, then right-click again. This will establish a connection between the two nodes, indicated by the blue arrow. Clicking on the node at the tail of the arrow will show that two new text fields have appeared at the bottom of the editor, under the text "Edit the response options". The first such text field is the dialog that the player will speak to the NPC in response to the text in the textfield in the bottom left corner. The text field immediately below this should be the exact name of a function that you would want to invoke when the player chooses this reponse option in game (for instance __myFunction()__). This field is optional. You can delete connections between nodes by pressing the red X beside the corresponding response text field. To delete a node, clicking on it will reveal a red X at the top of the editor that will delete a node _and all of its connections to other nodes_.
