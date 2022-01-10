### Update list
`v6.1.0.1`
*  corrections in translation

`v6.1.0.0`
*  Added stack control. Use `NotificationConstants.IsReversedPanel` to change stack orientation. Set as `null to default`.
* Message position as `Absolute` with constant setting: `NotificationConstants.AbsolutePosition`

`v6.0.0.0`
* Added net6 supporting

`v5.7.1.2`
* fix text settings for string only messages

`v5.7.1`
* Remove FontAwesome dependency
* Corrects message height calculation when font style changes to non-standart
* Fix SwgAwesome STA error whith progress
  
`v5.7.0`
* Add new sample project in repository
* Add Center screen message position
* Fix progress margin style when no waiting message and button
* Add functions for base message functions (errors view, open file or directory)

  
`v5.6.1`
* Added Image template for Notification content
  
`v5.6.0`
* Added overlay window position from constants,
* ImageSource template for icon,
* X-Close button visible setting
* Add Notification Min and Max Setting

`v5.5.0`
* Added text style settings to content

`v5.4.0`
* Added section for image
* if icon is null - left column will collapse

`v5.3.0`
* access to basic message settings
* setting the number of messages in the window
* settings for the progress bar  to automatically collapse with a large number of messages
* settings progress bar styles
* New progress bar initializing template
* Add more sample in test project

[Constants](https://github.com/Platonenkov/Notification.Wpf/blob/dev/Notification.Wpf/Constants/NotificationConstants.cs)

Just change it befor first using

`v5.2.0`
* Color and Icons in settings

`v5.1.0`
* Add Slowed progress bar
* Add Waiting timer for progress bar

`v5.0.0`
* Platform support
