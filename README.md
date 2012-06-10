Combine
=======

----------
###Motivation 1.0: CSS  

For performance reasons, it's usual to have only one ".css" file on the server with all the necessary styles for a site. This is done to avoid the browser making several calls to the server in order to download several ".css" files. Because the sum of the latency time for each call ends up being higher than the time necessay to download a unique larger ".css" file.

This issue is not present during development on local machines. Even worse! By having a unique ".css" file on the project, sooner or later it will have hundred's and thousand's of lines. Even using comments, it becomes impractical to manage all the CSS styles. 
 
Keeping the styles separated in several files or even groupping these in folders allows for an easier management. This way we can separate our styles by webstie areas, specific controls/classes, etc.

Each individual ".css" file becomes smaller it's contents easier to manage and so this also eases reutilization for other websites or diferent parts of a website.

Another benefit of having the styles structured this way is that their structure becomes visible and we can more easely find and access the styles we are looking for. You can think of it like an index. 

----------
###Problem: How to make the transition from development to production?

By making use of an already existent feature of CSS!  

The @import directive already allows a page to link to only one ".css" file which can then import other ".css" files. But, it's still the job of the browser to make a call to the server to download each imported stylesheet, which leaves us at the initial problem, performance.

So, instead of leaving this work to the browser, during a build we can parse an initial ".css" file and substitute each @import directive with the contents of the file it points to and feed that to the server and so the browser.  

This way we are actually **combining** several stylesheets into one. And doing some pre-processing so the server and browser don't have to. 

Now, what if...?  

- The stylesheet that is imported also has other @import directives?  
- There's a cyclic set of imports? The process would never end...
- In the tree of imports, the same stylesheet was imported more than once? There would be waste that would increase the size of the final file.

So, we need to make sure that:  

- The process is recursive.
- We check that we only import a file once during the whole process, discarting duplicates. 

----------
###Motivation 2.0: Regex for everyone

Soon to come...