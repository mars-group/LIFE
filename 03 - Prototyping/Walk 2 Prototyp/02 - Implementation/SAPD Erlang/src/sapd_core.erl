%% Copyright
-module(sapd_core).
-author("Christian Hüning").



%% API
-export([start/0]).

start() ->
  {ok, ConfigList} = file:consult("configs/properties.cfg"),
  {ok, PDCMcastAddr} = util:get_config_value(pdcmcastaddr, ConfigList),
  {ok, PDCMcastPort} = util:get_config_value(pdcmcastport, ConfigList),

  {ok, Frequency} = util:get_config_value(frequency, ConfigList),
  {ok, Stepwidth} = util:get_config_value(stepwidth, ConfigList),

  %% Initialize Counter through Controller
  SAPDCounter = sapd_pcounter_controller:start(Frequency,Stepwidth),

  %% Parameters initialization
  P = 0,
  PDC = 0,
  F = 1000,
  FT = 0,
  FB = 0,
  I = 0,
  Lambda = 1,
  Paverage = -1,
  { ok, SendSocket } = gen_udp:open(0),



  spawn(fun() -> loop(SAPDCounter, PDC, P, F, FT, FB, I, Lambda, Paverage, SendSocket, PDCMcastAddr, PDCMcastPort) end).

loop(SAPDCounter, PDC, P, F, FT, FB, I, Lambda, Paverage, Socket, PDCMcastAddr, PDCMcastPort) ->

  %% get current PDC and P
  SAPDCounter ! {getPDCandP, self()},
  receive
    {NewPDC, NewP} ->

      NewPaverage = -1,

      {NewF, NewerPaverage} = while_not_calibrated(SAPDCounter, NewPDC, NewP, F, FT, FB, I, Lambda, NewPaverage, Socket, PDCMcastAddr, PDCMcastPort),
      loop(SAPDCounter, NewPDC, NewP, NewF, FT, FB, I+1, Lambda, NewerPaverage, Socket, PDCMcastAddr, PDCMcastPort);

    _Anything ->
      io:format("Received: ~p \n",[_Anything]),
      loop(SAPDCounter, PDC, P, F, FT, FB, I, Lambda, Paverage, Socket, PDCMcastAddr, PDCMcastPort)
  end.


while_not_calibrated(SAPDCounter, PDC, P, F, FT, FB, I, Lambda, Paverage,Socket, PDCMcastAddr, PDCMcastPort)
  when abs(P - Paverage) /= 0  ->

    {PDCNodes, PNodes} = getPFromAllNodes(Socket, PDCMcastAddr, PDCMcastPort),

    Pn = util:average(PDCNodes),


    NewLambda = Pn div PDC,


    NewF = Lambda * F,
    if
      NewF /= 0 ->
        SAPDCounter ! {setFrequency, NewF};  %% set new Frequency to Counter only if NewF is not zero. Just happens if there's only one node
      true -> 1
    end,



    %%Inhalt = lists:concat(["P: ",P,
    %%                       ", Paverage: ",Paverage,
    %%                       ",\n PN: ", Pn, "\n", "Freq: ", NewF, "\n", "OldFreq: ", F, ", Lambda:",Lambda," \n\n\n"]),
    %%util:logging("while.log",Inhalt),
    %%util:logstop(),

    NewPaverage = util:average(PNodes),

    while_not_calibrated(SAPDCounter, PDC, P, NewF, FT, FB, I, NewLambda, NewPaverage, Socket, PDCMcastAddr, PDCMcastPort);

while_not_calibrated(_SAPDCounter, _PDC, _P, F, _FT, _FB, _I, _Lambda, Paverage, _Socket, _PDCMcastAddr, _PDCMcastPort) ->
  {F, Paverage}.


getPFromAllNodes(SendSocket, PDCMcastAddr, PDCMcastPort) ->
  gen_udp:send(SendSocket, PDCMcastAddr, PDCMcastPort, term_to_binary({getPDCandP, self()})),
  rcvPFromAllNodes([], [], length(nodes())).


rcvPFromAllNodes(ListPDC, ListP, Count) ->
  if
    Count > 0 ->
      receive
        {newPDCandP, PDC, P} ->
          rcvPFromAllNodes(lists:append([ListPDC, [PDC]]), lists:append([ListP, [P]]), Count-1);

        _Anything -> getPFromAllNodes(ListPDC, ListP, Count)
      end;

    %% Received all PDCs, return list.
    true ->
      {ListPDC, ListP}
  end.
