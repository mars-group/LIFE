{application,nodefinder,
             [{description,"Erlang multicast node discovery service."},
              {vsn,"0.2.1"},
              {modules,[nodefinder,nodefindersrv,nodefindersup]},
              {registered,[nodefindersrv]},
              {applications,[kernel,stdlib,crypto]},
              {env,[{addr,{239,123,4,1}},{port,5555},{multicast_ttl,1}]},
              {mod,{nodefinder,[]}}]}
.