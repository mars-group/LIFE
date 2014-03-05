%% Copyright
-module(zptc).
-author("Christian Hüning").

%% zptc = Zero Point Time Calibration
%% Steps:
%% 1) Sends w (=10) messages to all nodes.
%% 2) Receiving nodes measure time difference between their local receiving time of the current and the previous message
%% 3) After 10 received messages, each node calculates the probable receiving time of the 11th message, sets a timer for
%% that time and then starts the SAPDCore

%% API
-export([start/0, stop/1]).

%% Public

start() ->
  %%net_kernel:start([chdesktop, shortnames]),
  %%erlang:set_cookie(node(),zummsel),

  %% read Multicast Configuration from Configfile


  {ok, ConfigList} = file:consult("configs/properties.cfg"),
  {ok, ZPTCMcastAddr} = util:get_config_value(zptcmcastaddr, ConfigList),
  {ok, ZPTCMcastPort} = util:get_config_value(zptcmcastport, ConfigList),

  %% Start nodefinder
  application:load(nodefinder),
  application:load(crypto),
  application:start(crypto),
  application:start(nodefinder),


  %% discover all other nodes via multicast
  nodefinder:discover(),

  %% Start receiver for Zero Point Time Calibration Multicasts
  S = open(ZPTCMcastAddr, ZPTCMcastPort),
  Pid = spawn(fun() -> receiver([],0,0,0) end),
  gen_udp:controlling_process(S,Pid),
  {S,Pid}.



stop({S, Pid}) ->
  close(S),
  Pid ! stop.


%% Private

%% Will receive 10 messages, collect their Deliverytime in a list,
%% calculates the average of these times and starts a timer to start SAPDCore.
receiver(List, Count, PreviousStamp, CurrentStamp) ->
  if
    Count < 10 ->
      receive
        {udp, _Socket, _IP, _InPortNo, _Packet} ->
          NewCurrentStamp = erlang:now(),
          if
            PreviousStamp /= 0 ->
              MessageDeliveryTime = timer:now_diff(NewCurrentStamp, PreviousStamp) div 1000,
              receiver(lists:append(List, [MessageDeliveryTime]), Count+1, NewCurrentStamp, PreviousStamp);

            true ->
              receiver(lists:append(List), Count+1, NewCurrentStamp, PreviousStamp)
          end;


        stop -> true;

        AnythingElse -> io:format("RECEIVED: ~p~n",[AnythingElse]),
          receiver(List,Count,PreviousStamp,CurrentStamp)
      end;

    %% All messages received, start SAPDCore after TimeToStart
    true ->
      TimeToStart = abs(util:average(List)) div 1000,
      timer:sleep(TimeToStart),
      sapd_core:start()
  end.


open(Addr, Port) ->

  IPAddr = util:get_main_ipadress(),

  Opts = [ { active, true },
    { ip, IPAddr },
    { add_membership, { Addr, IPAddr } },
    { multicast_ttl, 4 },
    { multicast_loop, true },
    { reuseaddr, true },
    binary ],

  {ok,S} = gen_udp:open(Port,Opts),
  S.

close(S) -> gen_udp:close(S).