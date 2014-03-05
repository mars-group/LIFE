-module(util).
-author("Prof. Dr. Klauck, Christian Hüning").

%% API

-export([get_config_value/2, logging/2, logstop/0, timeMilliSecond/0, delete_last/1, shuffle/1, reset_timer/3, type_is/1, for/2, get_main_ipadress/0, average/1]).
-define(ZERO, integer_to_list(0)).


get_main_ipadress() ->
  {ok, Hostname} = inet:gethostname(),
  {ok, IPAddr} = inet:getaddr(Hostname, inet),
  IPAddr.

%
% for-loop
%
for(N, Fun) when is_integer(N), is_function(Fun, 1) ->
  util:for({N, 0}, Fun)
;
for({N, _}, _) when is_integer(N), N < 1 ->
  ok
;
for({N, LoopCount}, Fun) when is_integer(N), is_function(Fun, 1) ->
  Fun(LoopCount),
  util:for({N-1, LoopCount+1}, Fun)
.

%% Average of a list
average(X) ->
  average(X, 0, 0).

average([H|T], Length, Sum) ->
  average(T, Length + 1, Sum + H);

average([], Length, Sum) ->
  if
    Length /= 0 ->  Sum div Length;
    true -> 0
  end.

%% -------------------------------------------
% Werkzeug
%%
%% Sucht aus einer Config-Liste die gewünschten Einträge
% Beispielaufruf: 	{ok, ConfigListe} = file:consult("server.cfg"),
%                  	{ok, Lifetime} = get_config_value(lifetime, ConfigListe),
%
get_config_value(Key, []) ->
  {nok, Key};
get_config_value(Key, [{Key, Value} | _ConfigT]) ->
  {ok, Value};
get_config_value(Key, [{_OKey, _Value} | ConfigT]) ->
  get_config_value(Key, ConfigT).

% Schreibt auf den Bildschirm und in eine Datei
% nebenläufig zur Beschleunigung
% Beispielaufruf: logging('FileName.log',"Textinhalt"),
%
logging(Datei,Inhalt) -> Known = erlang:whereis(logklc),
  case Known of
    undefined -> PIDlogklc = spawn(fun() -> logloop(0) end),
      erlang:register(logklc,PIDlogklc);
    _NotUndef -> ok
  end,
  logklc ! {Datei,Inhalt},
  ok.

logstop( ) -> 	Known = erlang:whereis(logklc),
  case Known of
    undefined -> false;
    _NotUndef -> logklc ! kill, true
  end.

logloop(Y) -> 	receive
                 {Datei,Inhalt} -> io:format(Inhalt),
                   file:write_file(Datei,Inhalt,[append]),
                   logloop(Y+1);
                 kill -> true
               end.

%% Löscht das letzte Element einer Liste
% Beispielaufruf: Erg = delete_last([a,b,c]),
%
delete_last([]) -> [];
delete_last([_Head]) -> [ ];
delete_last([Head|Tail]) -> [Head|delete_last(Tail)].

% schneller:
% delete_last(List) ->
%   [_|B] = lists:reverse(List),
%   lists:reverse(B).

%% Mischt eine Liste
% Beispielaufruf: NeueListe = shuffle([a,b,c]),
%
shuffle(List) -> shuffle(List, []).
shuffle([], Acc) -> Acc;
shuffle(List, Acc) ->
  {Leading, [H | T]} = lists:split(random:uniform(length(List)) - 1, List),
  shuffle(Leading ++ T, [H | Acc]).

%%
% Unterbricht den aktuellen Timer
% und erstellt einen neuen und gibt ihn zurück
%%
reset_timer(Timer,Sekunden,Message) ->
  {ok, cancel} = timer:cancel(Timer),
  {ok,TimerNeu} = timer:send_after(Sekunden*1000,Message),
  TimerNeu.

%% Zeitstempel: 'MM.DD HH:MM:SS,SSS'
% Beispielaufruf: Text = lists:concat([Clientname," Startzeit: ",timeMilliSecond()]),
%
timeMilliSecond() ->
  {_Year, Month, Day} = date(),
  {Hour, Minute, Second} = time(),
  Tag = lists:concat([klebe(Day,""),".",klebe(Month,"")," ",klebe(Hour,""),":"]),
  {_, _, MicroSecs} = now(),
  Tag ++ concat([Minute,Second],":") ++ "," ++ toMilliSeconds(MicroSecs)++"|".
toMilliSeconds(MicroSecs) ->
  Seconds = MicroSecs / 1000000,
  string:substr( float_to_list(Seconds), 3, 3).
concat(List, Between) -> concat(List, Between, "").
concat([], _, Text) -> Text;
concat([First|[]], _, Text) ->
  concat([],"",klebe(First,Text));
concat([First|List], Between, Text) ->
  concat(List, Between, string:concat(klebe(First,Text), Between)).
klebe(First,Text) ->
  NumberList = integer_to_list(First),
  string:concat(Text,minTwo(NumberList)).
minTwo(List) ->
  case {length(List)} of
    {0} -> ?ZERO ++ ?ZERO;
    {1} -> ?ZERO ++ List;
    _ -> List
  end.

% Ermittelt den Typ
% Beispielaufruf: type_is(Something),
%
type_is(Something) ->
  if is_atom(Something) -> atom;
    is_binary(Something) -> binary;
    is_bitstring(Something) -> bitstring;
    is_boolean(Something) -> boolean;
    is_float(Something) -> float;
    is_function(Something) -> function;
    is_integer(Something) -> integer;
    is_list(Something) -> list;
    is_number(Something) -> number;
    is_pid(Something) -> pid;
    is_port(Something) -> port;
    is_reference(Something) -> reference;
    is_tuple(Something) -> tuple
  end.