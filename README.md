# Bowyer-Watson Delaunay Triangulation
<br>
<img src="https://user-images.githubusercontent.com/33559521/229410603-e8b349a7-dc0e-46c3-b32f-91a97fe0fa18.png" width="250px" height="250px"/>
<br>
                                                                                                                     
                                                                                                           
This is a simple implementation of the Bowyer-Watson Deluanay Triangulation. It is an incremental algoritm so it isn't particularly fast.

The algorithm is **data orientated**, so all of the data types are using structs instead of classes. And the API is engine agnostic but I have added a demo scene via Unity3D.

I have also added support for <a href="https://docs.unity3d.com/Manual/JobSystem.html">Unity's Job System</a> to allow the use of threads, but it isn't ideal for this type of algoritm due to its incremental nature. 
I might try a new approach to using the job system for this algorithm in future when I have time to optimise.

### Note
This implementation is not fully optimised as much as it could be. The aim was to learn the algoritm not make it highly performant.

## Preview

Here is a Unity3D preview of the triangulated mesh being generated whilst the points move randomly. The triangulation runs every frame after the points have been moved to re-triangulate the mesh. Additionally moving the mouse over the mesh will act as an additional point to be traingulated which is highlighted in purple with adjacent triangules within a certain distance being highlighted in purple aswell.

<br>
<img src="https://user-images.githubusercontent.com/33559521/230699882-567636c1-09d2-4d66-a9c1-020ede678df9.gif" width="40%" height="40%"/>


## Sources 

Helpful sources to learn the algorithm:

<a href="https://en.m.wikipedia.org/wiki/Delaunay_triangulation">Wikipedia :: Delaunay Triangulation</a>
<br>
<a href="https://en.m.wikipedia.org/wiki/Circumscribed_circle">Wikipedia :: Circumscribed Circle</a>
<br>
<a href="https://en.m.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm">Wikipedia :: The Bowyer-Watson Algorithm </a>
<br>
<a href="https://github.com/TheCelticGuy/Triangulation/files/11186438/07_An-implementation-of-Watsons-algorithm-for-computing-two-dimensional-Delaunay-triangulations.pdf">PDF :: An implementation of Watson's algorithm for computing
2-dimensional Delaunay triangulations 
</a>
<br>
<a href="https://youtu.be/4ySSsESzw2Y">YouTube :: A visual Guide by Scott Anderson</a>
