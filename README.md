# FS4520-CheckSpec
With this software, a user can code their own software to write validation tests on Display Port. Used in conjunction with the FS4500.<br/>

The following project contains the source code for the check spec. The goal is for a user to make changes to meet their testing needs. Below are instructions to get started<br/>

Instructions:<br/>

Moving dlls:<br/>
   -The dll in each of the folders of this repository must be moved to the following location C:\Users\userName\Documents\FuturePlus\FS4500\VTests. <br/>
   -The FS4500 should be created when the FS4500 is installed with the VTests folder. If missing the user can add a new folder named VTests.<br/>
   -The dlls needed are DP14ValidationAttributes.dll, DP14ValidationTestsInterface.dll, FS4500_VTests_ML_Functions.<br/>
   -FS4500_ML_FramingValidation.dll are some example tests for the user to look at.<br/>
   
Changing Source Code:<br/>
   -The source code for the dlls are included within this respository.<br/>
   -The user can make changes to the source code and overwrite a dll. The new dll must be named to same as the dll it is replacing.<br/>
   -These dlls were compiled using Visual Studios.  <br/>
