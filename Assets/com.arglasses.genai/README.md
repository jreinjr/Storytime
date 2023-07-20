This is a package that has MetaGen calls put together by David Geisert on 13 July 2023.
This is not an official release from the MetaGen team.
David Geisert is not a member of the MetaGen team.
Do not reach out to the MetaGen team if this breaks, reach out to David Geisert.

To get an access token go to https://www.internalfb.com/intern/oauth and click on MetaGen > Generate Token

Put AIManager in your scene and add your access token to it.

Take a look at the example usage or the tests.  
I'm relying on UnityEvents as callbacks.  
The passthroughData can be a JSON string or just a simple string.  
The primary purpose of passthroughData is running multiple calls in parallel through the same callbacks.  
If you have multiple images you want for a collage you can pass the position in the collage and then in the callback look at the passed through image information.


Currently MetaGen HTTP endpoints require internal access.  This means the device must be on VPN, Lighthouse, Lab Networks, or Kittyhawk.
I'll try to get the packages to use GraphQL, but that isn't expected by the July GenAI hackathon.