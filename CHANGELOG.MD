### Changelog

All notable changes to this project will be documented in this file. Dates are displayed in UTC.

Generated by [`auto-changelog`](https://github.com/CookPete/auto-changelog).

#### [v0.2.6](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.6-0...v0.2.6)

- remove unlisten [`e219ecd`](https://github.com/ChrisAshtear/EUIDatasources/commit/e219ecd30572fef86d40077213052316aa01ff69)

#### [v0.2.6-0](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.5...v0.2.6-0)

> 28 April 2023

- initial move to DataItem [`76c7c53`](https://github.com/ChrisAshtear/EUIDatasources/commit/76c7c53c683736bce76322f6d5192c442697e37e)
- item drop downs working in editor. cant get item from it yet [`05d07b0`](https://github.com/ChrisAshtear/EUIDatasources/commit/05d07b04d8397240aa785187784e98d1b590b806)
- DataItem [`096b9bb`](https://github.com/ChrisAshtear/EUIDatasources/commit/096b9bb486ba713b30490748cefbad1fa6be4c65)

#### [v0.2.5](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.4...v0.2.5)

> 13 November 2021

- support scriptable object data type. [`51acd42`](https://github.com/ChrisAshtear/EUIDatasources/commit/51acd426df934bc3e672c9dffa61530966bcd018)
- Had to readd subtype field in data tag to be able to use showifequal type. required for car derby equipping. more error details. [`3c9b7d6`](https://github.com/ChrisAshtear/EUIDatasources/commit/3c9b7d652f9cbe94aa2c7a1a57469b095399289e)
- Release 0.2.5 [`c150ba0`](https://github.com/ChrisAshtear/EUIDatasources/commit/c150ba04d5b3bd5786224fcf6f42e61bbf1cf52e)

#### [v0.2.4](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.3...v0.2.4)

> 4 August 2021

- updated to work with eui base update 0.1.11 moving scripts from guiutil to edguiutil [`6922d98`](https://github.com/ChrisAshtear/EUIDatasources/commit/6922d9829f6b561225c7f672d2191f51779c3776)
- CHANGELOG.MD [`5245391`](https://github.com/ChrisAshtear/EUIDatasources/commit/52453918ef17eef02889d50e2619d3adae674da2)
- messed up version - fixed [`96b49e0`](https://github.com/ChrisAshtear/EUIDatasources/commit/96b49e09b1f964a8e7d22886ba508f648721c248)

#### [v0.2.3](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.2...v0.2.3)

> 4 August 2021

- added clear table function and both it and drop table call datachanged events. (DatabaseSource) [`4dc814e`](https://github.com/ChrisAshtear/EUIDatasources/commit/4dc814e44fcba368a6fb799efb10c98fad77c032)
- CHANGELOG.MD [`a659eb9`](https://github.com/ChrisAshtear/EUIDatasources/commit/a659eb971a232dfe5a5793dab4d7df906667e745)
- DBsource now has clearTable which calls datachanged event, UIShowAttributes can be called manually instead of automatically. [`9f11980`](https://github.com/ChrisAshtear/EUIDatasources/commit/9f1198010661ea14abcb2656e50688f922768f4d)

#### [v0.2.2](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.1...v0.2.2)

> 31 July 2021

- was missing a prefab property in populate table editor, changed samples because they werent properly linked to each other [`b59be1b`](https://github.com/ChrisAshtear/EUIDatasources/commit/b59be1bdd46a34c8d6f349f1b022cba0890d02f5)
- CHANGELOG.MD [`5a6303c`](https://github.com/ChrisAshtear/EUIDatasources/commit/5a6303c1e61836e794f005fcc1bef0448cd58f9f)
- was missing a prefab property in populate table editor, changed samples because they werent properly linked to each other [`42d42be`](https://github.com/ChrisAshtear/EUIDatasources/commit/42d42be6f4b63a91f0fe8134f02059e9d23d144c)

#### [v0.2.1](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.2.0...v0.2.1)

> 31 July 2021

- CHANGELOG.MD [`02ad70b`](https://github.com/ChrisAshtear/EUIDatasources/commit/02ad70bc904a2a84285e82cca16dc742d28b2900)
- updated base EUI version requirement [`14778f3`](https://github.com/ChrisAshtear/EUIDatasources/commit/14778f3c565e52aedea1b32f80a802bb8b573177)
- updated eui version req [`6ccec9c`](https://github.com/ChrisAshtear/EUIDatasources/commit/6ccec9cad63b0c36c13dd2c6cb8ff26aa06fb1ca)

#### [v0.2.0](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.1.9...v0.2.0)

> 31 July 2021

- All items work - data dropdown, poptable, poplist. Custom editor for poptable. Made sample scene [`1254884`](https://github.com/ChrisAshtear/EUIDatasources/commit/125488441d3c54e105355a8e8006a31e333e94bf)
- added intermediate class to handle loading db data from file or web(doesnt support streaming assets currently), and added editor script to only display relevant properties when a db is marked as local or web [`970c4db`](https://github.com/ChrisAshtear/EUIDatasources/commit/970c4db94eac3c36fbab93b1e210ec77b1274a75)
- removed unused sources,updated DBloader to support loading from streaming assets, updated editor for db source to reflect this. [`1b922c6`](https://github.com/ChrisAshtear/EUIDatasources/commit/1b922c637c90802d28bb305cf5e03a14bc334a63)

#### [v0.1.9](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.1.8...v0.1.9)

> 21 July 2021

- Added a way to send data out to the web with ui form, now can load sprites from file. added json dependency. [`5053ccc`](https://github.com/ChrisAshtear/EUIDatasources/commit/5053cccaf52e05fb70020fb3cd21208b0cf94096)
- Added UI Form to send data to remote source. [`d7701a3`](https://github.com/ChrisAshtear/EUIDatasources/commit/d7701a3b2312d16059a1b7cbdefede3f55814b1a)

#### [v0.1.8](https://github.com/ChrisAshtear/EUIDatasources/compare/v0.1.7...v0.1.8)

> 19 July 2021

- changelog [`b85e534`](https://github.com/ChrisAshtear/EUIDatasources/commit/b85e5348c28587ac590bf61eac1665f12821dbd3)
- bump [`f61044d`](https://github.com/ChrisAshtear/EUIDatasources/commit/f61044dad37b0b694d34a72e783bf4549fdd4f40)

#### v0.1.7

> 19 July 2021

- Added scene and updated version [`a7968ff`](https://github.com/ChrisAshtear/EUIDatasources/commit/a7968ff71dcb2d092e15990418b8400b42adc84d)
- Initial Commit [`cf7693d`](https://github.com/ChrisAshtear/EUIDatasources/commit/cf7693d70be5a9c5b4dbdd4ec0494fb4746ccf98)
- Support 'attributes', properties that can be in a table. [`42a1411`](https://github.com/ChrisAshtear/EUIDatasources/commit/42a1411f199f7005306a15154ff0e3b9dfca57e7)
