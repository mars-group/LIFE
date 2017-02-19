# LIFE v2.1

A re-structured LIFE solution that unites the LIFE-API with the model-development components and the simulation runtime.

### Contents
* LIFE-API: The former 'LifeAPI'. Contains all LIFE interfaces.
* LIFE-Components: All optional components regarding base/predefined components for model development.
  * Agents
        * DalskiAgent 
  * Layers
        * ObstacleLayer
        * PotentialFieldLayer
        * TimeSeriesLayer
  * Environments
        * EnvironmentServiceComponent
        * GeoGridEnvironment
  * Utilities
        * CloudSupport
* LIFE-Core: The LIFE Core components LayerContainer and SimulationManager
* Tests: All component-related tests are gathered here.

### Versioning

All components are detached from the `dotnetcore` or `master` branches of their repositories and because of the required dependency changes, namespace renamings and formattings, it seems hardly possible to automatically merge any new changes. Probably the easiest way would be to apply the updates manually in regular intervals, based on the diffs of the origin and merged version.

The latest version of LIFE v2.1 rests on these commits:


| Repository                  | Current date  | Merged commit                              |
|-----------------------------|------------|--------------------------------------------|
| [CloudSupport](https://gitlab.informatik.haw-hamburg.de/mars/overview_move_to_gitlab/commits/master)                               | 06.02.2017 | `aea928ef27b7924c4e92dfe437255d53bcb7eed8` |
| [DalskiAgent](https://gitlab.informatik.haw-hamburg.de/mars/life-dalskiagent/commits/master)                                       | 23.01.2017 | `cf312336e606e23fec3d25920f0f8271278412bc` |
| [EnvironmentServiceComponent](https://gitlab.informatik.haw-hamburg.de/mars/life-environment-service-component/commits/dotnetcore) | 05.11.2016 | `d451f78f577362670a845a5d5ba624c1fff8e1c2` |
| [GeoGridEnvironment](https://gitlab.informatik.haw-hamburg.de/mars/life-geo-grid-environment/commits/dotnetcore)                   | 06.02.2017 | `f9c2e087a5769ea4156330214ce620626f13539e` |
| [LIFE](https://gitlab.informatik.haw-hamburg.de/mars/life/commits/LIFEv2)                                                          | 15.02.2017 | `8d70be65c0e2a3eb00034b158adfb3952c00224e` |
| [ObstacleLayer](https://gitlab.informatik.haw-hamburg.de/mars/life-obstacle-layer/commits/master)                                  | 06.02.2017 | `90e98b2a55d6f7c36cd3ffc76a8463d28d65d1ba` |
| [PotentialFieldLayer](https://gitlab.informatik.haw-hamburg.de/mars/life-potential-field-layer/commits/master)                     | 06.02.2017 | `2a5c6104eb1b94bc0a7443bca26057d07f7cdb19` |
| [TimeSeriesLayer](https://gitlab.informatik.haw-hamburg.de/mars/life-time-series-layer/commits/dotnetcore)                         | 06.02.2017 | `7fab686db9b8ee522e2ea01a79caa8e78cdf36e3` |
