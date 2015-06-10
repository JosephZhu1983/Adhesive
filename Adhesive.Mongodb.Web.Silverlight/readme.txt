THEMING
When defining your own styles, set "basedon" to be “Default(Control Type)Style” to keep the theme working

The Assets folder contains resource dictionaries used for visual styling:
Brushes.xaml contains all color and brush resources used in styles
Fonts.xaml contains all font family and size information used in styles
Styles.xaml contains the keyed styles used by MainPage.xaml and content pages
CoreStyles.xaml contains implicit styles with control templates for all core controls
SDKStyles.xaml contains implicit styles with control templates for all SDK controls
ToolkitStyles.xaml contains implicit styles with control templates for all toolkit controls
   ToolkitStyles.xaml is set to a build action of "none" as toolkit assemblies are not included in the template
   If you will not use toolkit controls, please delete the file
   To enable toolkit controls, please follow the directions in app.xaml

To use the theme but not the navigation framework, delete the Views folder, delete Styles.xaml, and reset MainPage to a blank user control

NAVIGATION FRAMEWORK
To add pages to mainpage.xaml:
1. Create a new Page in the Views folder
2. Copy / paste a navigation button on mainpage.xaml
3. Select the copied button and update the Content property to be the button name
4. Update the NavigateURI property to be the page name in the views folder
   Note that NavigateURI uses urimapper to eliminate the folder name and extension of the new page,
   so that a friendly URI is shown in the browser or for deep linking

The Views folder contains samples for some common content page types.
Keep the ErrorWindow.xaml and Home.xaml pages, as these are integrated into the navigation framework.
Other pages may be deleted or renamed if you do not find them useful.

See here for a preview of all controls:
http://www.silverlight.net/content/samples/sl4/themes/cosmopolitan.html 
