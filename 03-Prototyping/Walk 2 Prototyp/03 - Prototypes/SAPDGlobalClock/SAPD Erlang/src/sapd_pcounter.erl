%% Copyright
-module(sapd_pcounter).
-author("Chris").


%% API
-export([start/3]).

%% The SAPD-P-Counter. After each increase-step waits for StepWidth Interval
%% to receive a new frequency

start(Frequency, StepWidth, Controller) ->
  spawn(fun() -> increasePDC(Frequency, StepWidth, 0, Controller) end).

    %% Controller überflüssig!
increasePDC(Frequency, StepWidth, Counter, Controller) ->
  %% PDC timing model: Pn+1 = Pn + F*Delta(T)
  NewCounter = Counter + Frequency * StepWidth,
 %% Controller ! {setCurrentPDC, NewCounter, NewCounter - Counter},

  receive
    {setFrequency, NewFrequency} ->
      increasePDC(NewFrequency, StepWidth, NewCounter, Controller);

    {getCurrentPDCandP, Caller} ->
      Caller ! {setPDCandP, NewCounter, NewCounter - Counter},
      increasePDC(Frequency, StepWidth, NewCounter, Controller)

  after StepWidth ->
    increasePDC(Frequency, StepWidth, NewCounter, Controller)
  end.

