Dear Developer,

Thank you for your support.

Static Sprites makes possible static batching of sprites, greatly improving performance when rendering non-moving sprites. Now, you can render thousands of non-moving sprites on even low-end mobile devices.

As a technical background, Unity normally dynamically batches the rendering of sprites as an optimization. Dynamic batching means it groups the geometry into similar renderable batches each frame, and this generally works well for small moving geometry. However, dynamic batching is a non-optimal solution for non-moving sprites, because the CPU still does a lot of work constructing the batches each frame! If you open the Unity Profiler, you can check the Dynamic Batching batched draw calls count -- this is all redundant work since the sprites don't move.

Static batching on the other hand, is much faster as it pre-generates the batches and geometry to be rendered just once. Unity's static batching system however, only works for meshes. Toggling the Static checkbox on sprites does nothing, unfortunately.

That's where Static Sprites comes in. Simply attach the component to any game object, and any sprite that is a child in the hierarchy under the object is statically batched on start. It does this automatically, intelligently combining sprites into singular meshes while respecting differing materials, textures, atlasing, colors, sorting orders, or Flip XY direction. There is additionally a 'Export combined sprites' should you need to save out the combined sprites to a folder.

And that's it, I hope you love it!
If you have any questions, please drop us a line:
Discord: https://discord.gg/mWM3aTWVdG
Unity Forum: https://discussions.unity.com/t/released-static-sprites/934816

We can't wait to see what you'll create next!

OmniShade
contact@omnishade.io