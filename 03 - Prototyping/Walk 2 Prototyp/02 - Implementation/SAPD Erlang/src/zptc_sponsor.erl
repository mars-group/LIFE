%% Copyright
-module(zptc_sponsor).
-author("Christian Hüning").

%% API
-export([start/0]).


start() ->
  {Socket,RecvPid} = zptc:start(),

  {ok, ConfigList} = file:consult("configs/properties.cfg"),
  {ok, ZPTCMcastAddr} = util:get_config_value(zptcmcastaddr, ConfigList),
  {ok, ZPTCMcastPort} = util:get_config_value(zptcmcastport, ConfigList),

  spawn(fun() -> send_pulse(send_socket(), ZPTCMcastAddr, ZPTCMcastPort, 0) end),
  {Socket,RecvPid}.

send_pulse(Socket, ZPTCMcastAddr, ZPTCMcastPort, Count) ->
  if
    Count < 10 ->
      timer:sleep(500),
      gen_udp:send(Socket, ZPTCMcastAddr, ZPTCMcastPort, atom_to_binary(hello, latin1)),
      send_pulse(Socket, ZPTCMcastAddr, ZPTCMcastPort, Count+1);

    true ->
      gen_udp:close(Socket),
      done
  end.

send_socket() ->
  { ok, SendSocket } = gen_udp:open(0),

  SendSocket.