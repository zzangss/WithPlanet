-----------------
DEMO INSTRUCTIONS
-----------------
This demo is a scene of 1000 sprites, showcasing the advantages of using Static Sprites. 

There is a StaticSprites component on the '1000 Sprites' game object. When Play Mode is entered, it batches all the children sprites under it into a combined mesh for optimal rendering.

To check the performance:
1. First disable the StaticSprites component on '1000 sprites' so we can check the baseline performance.
2. Enter Play Mode
3. Either expand the Statistics widget in the Game View or open the Profiler (Menu->Window->Analysis). Note the number of batches saved by Dynamic Batching. It should be around 950.
4. Now, enable the StaticSprites component
5. Enter Play Mode again, and see the difference. There should now be no work done by Dynamic Batching!
6. This works with Sprite Atlases as well, so for even better results, ensure that atlasing is enabled from Project Settings->Editor->Sprite Packer.

Optional
7. If you need to save out the combined sprites to a folder, you can do this by pressing the 'Export combined sprites' button on the StaticSprites component.
