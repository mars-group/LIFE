%% Copyright
-module(sapd_pcounter_controller).
-author("Christian Hüning").

%% API
-export([start/2]).

%% The Controller to access and influence the SAPD-P-Counter


start(Frequency, StepWidth) ->
  Loop = spawn(fun() -> loop(0, 0, []) end),
  CounterPid = sapd_pcounter:start(Frequency, StepWidth, Loop),
  Loop ! {setCounterPid, CounterPid},
  register(sapd_controller, Loop),

  {ok, ConfigList} = file:consult("configs/properties.cfg"),
  {ok, PDCMcastAddr} = util:get_config_value(pdcmcastaddr, ConfigList),
  {ok, PDCMcastPort} = util:get_config_value(pdcmcastport, ConfigList),

  %% Start receiver for PDC Exchange Multicasts
  S = open(PDCMcastAddr,PDCMcastPort),
  Pid = spawn(fun() -> receiver(Loop) end),
  gen_udp:controlling_process(S,Pid),

  Loop.


loop(CurrentPDC, P, CounterPid) ->
  receive
    {setCounterPid, NewCounterPid} ->
      loop(CurrentPDC, P, NewCounterPid);

    {setFrequency, NewFrequency} ->
      CounterPid ! {setFrequency, NewFrequency},
      loop(CurrentPDC, P, CounterPid);

    {getPDC, Caller} ->
      CounterPid ! {getCurrentPDCandP, self()},
      receive
        {setPDCandP, NewCounter, NewP} ->
          Caller ! {NewCounter},
          loop(NewCounter, NewP, CounterPid)
      end;

    {getP, Caller}  ->
      CounterPid ! {getCurrentPDCandP, self()},
      receive
        {setPDCandP, NewCounter, NewP} ->
          Caller ! {NewP},
          loop(NewCounter, NewP, CounterPid)
      end;

    {getPDCandP, Caller}  ->
      CounterPid ! {getCurrentPDCandP, self()},
      receive
        {setPDCandP, NewCounter, NewP} ->
          Caller ! {NewCounter, NewP},
          loop(NewCounter, NewP, CounterPid)
      end;

     {keepalive, Caller} ->
       Caller ! {stillthere, CurrentPDC},
       loop(CurrentPDC, P, CounterPid)

  end.


receiver(Controller) ->
      receive
        {udp, _Socket, _IP, _InPortNo, Packet} ->
          io:format("received!"),
          {getPDCandP, CallerPid} = binary_to_term(Packet),
          Controller ! {getPDCandP, self()},

          receive
            {CurrentPDC, P} -> CallerPid ! {newPDCandP, CurrentPDC, P}
          end,

          receiver(Controller);

        stop -> true;

        AnythingElse -> io:format("RECEIVED: ~p~n",[AnythingElse]),
          receiver(Controller)
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


