# ${\textrm{\color{green}Bachelorarbeit-Demo}}$

Dieses Repository enthält eine ${\textrm{\color{orange}aufbereitete Demo-Version}}$ des Unity-Projekts, das meine Bachelorarbeit zum Thema $\textrm{\color{orange}Reinforcement Learning}$ in Unity begleitet. Es dient als kompakte, exemplarische Darstellung der wichtigsten Bestandteile. 

⚠️Hinweis: Im Vergleich zur Originalversion der Bachelorarbeit wurden größere Teile des ursprünglichen Codes und einige Komponenten entfernt und abgeändert. 

Das Readme selbst stellt eine kurze, kompakte Zusammenfassung der schriflichten Bachelorarbeit dar.


## ${\textrm{\color{red}1. Titel und Kurzbeschreibung }}$

- **Titel der Bachelorarbeit**  
  "Training und Evaluation eines ${\textrm{\color{orange}ML-Agenten}}$ zur Überwindung dynamischer Hindernisse und zur Lösung einfacher interaktiver Rätsel in Unity"

- **Motivation**  
  Traditionell manuell programmierte KI-Systeme, wie sie in modernen Computerspielen eingesetzt werden, stoßen schnell an ihre Grenzen, da sie in starren Handlungsmustern verharren und kaum flexibel auf neue Situationen reagieren. ${\textrm{\color{orange}Reinforcement Learning (RL)}}$  bietet dagegen das Potenzial, Agenten zu entwickeln, die durch Versuch und Irrtum selbstständig robuste und anpassungsfähige Strategien erlernen.

- **Problemstellung**  
  Klassische, manuell programmierte KI-Systeme in Spielen stoßen an Grenzen: Sie reagieren oft unflexibel auf unvorhergesehene Situationen und verallgemeinern schlecht auf neue Level. ${\textrm{\color{orange}Reinforcement Learning}}$ bietet zwar die Möglichkeit, Strategien durch Versuch und Irrtum zu erlernen, jedoch tendieren Agenten häufig dazu, nur spezifische Trainingsszenarien zu lösen, ohne ihr Verhalten auf neue Situationen übertragen zu können. Gleichzeitig kann eine zu große Vielfalt an Trainingsdaten den Lernprozess ineffizient oder instabil machen.

- **Zielsetzung**  
  Ziel der Arbeit ist die Entwicklung eines robusten ${\textrm{\color{orange}ML-Agenten}}$, der verschiedene dreidimensionale Parkour-Level mit hoher Erfolgsrate absolvieren kann. Untersucht wird insbesondere, wie Trainingsstrategien gestaltet sein müssen, damit der Agent nicht nur in bekannten Umgebungen stabil performt, sondern auch generalisierungsfähige Verhaltensweisen lernt, um neue Level, die nicht teil des Trainings waren,  erfolgreich zu bewältigen.

## ${\textrm{\color{red}2. Technische Anforderungen}}$


- **Game Engine und Tooling**
  - **Unity-Engine:** Original: $\color{pink}\textrm{2022.3.29f1}$, Demo: $\color{pink}\textrm{6000.2.7f2}$
  - **Unity** ${\textrm{\color{orange}ML-Agents Toolkit}}: \color{pink}\textrm{2.0.1}$
  - **Programmiersprachen:** C# (Unity), Python (Automatisierung Skripte)
  - **Code-Editor:** **Visual Studio Community 2022**, Versionen $\color{pink}\textrm{17.13.5 bis 17.14.19}$

- **Python-Umgebung**
  - Python: $\color{pink}\textrm{3.10.11}$
  - Virtuelle Umgebung: Projektinterner `venv`-Ordner mit allen benötigten Python-Paketen für ${\textrm{\color{orange}ML-Agents}}$ und das Automatisierungsskript

- **Trainings- und Simulationshardware**
  - Notebook: **Dell Inspiron 15 7510**
  - CPU: **Intel Core i7-11800H** (8 Kerne, 16 Threads, bis zu 4,6 GHz)
  - Arbeitsspeicher: **16 GB RAM**
  - GPU: **NVIDIA GeForce RTX 3050 Laptop GPU**
  - Hardwareleistung ausreichend für parallele Trainingsinstanzen und zufriedenstellende Trainingsgeschwindigkeit

- **Weitere eingesetzte Software / Tools**
  - ${\textrm{\color{orange}ML-Agents Python-CLI}}$ für das Training (`mlagents-learn`)
  - **TensorBoard** zur Visualisierung und Auswertung der Trainingsmetriken
  - Eigenes **Python-Automatisierungsskript** zur Steuerung von Trainingsläufen und Hyperparametern

## ${\textrm{\color{red}3. Architektur des Agenten }}$

### ${\textrm{\color{lightgreen}3.1 Zentrale Agentenkomponenten}}$

- **${\textrm{\color{orange}MLAgentController}}$**
  - Zentrales Steuerelement des Agentensystems
  - Verknüpft mehrere spezialisierte C#-Skripte zu einem Gesamtsystem
  - Implementiert Episodenlogik über `OnEpisodeBegin()`, `OnActionReceived()`, sowie Terminierungsmethoden `Die()` und `ReachGoal()`

- **Episoden- und Umgebungssteuerung**
  - **OnEpisodeBegin()**: Setzt den Agenten auf die Startposition zurück, randomisiert die Orientierung, setzt Belohnungen und Zähler zurück und aktiviert level-spezifische Umgebungskonfigurationen.
  - **EnvironmentController**: Schaltet je nach Level unterschiedliche Subsysteme zu, u. a. Bodenpositionsrandomisierung, Systeme für bewegliche Hindernisse und Checkpoint-Steuerung.
  - **TrackCheckpoints-System**: Verwalten der Checkpoints, Rücksetzen über `ResetCheckPoints()`, Aktivierung des ersten Checkpoints als Ziel.
  - Terminierung über
    - **Erfolg**: `ReachGoal()` (finales Ziel erreicht, positive Bewertung, visuelle Erfolgsmarkierung)
    - **Fehlschlag**: `Die()` bei MaxStep-Überschreitung, Kollision mit `killPlayer`-Objekten oder externen Triggern (negative Bewertung, rote Färbung des Agenten).

### ${\textrm{\color{lightgreen}3.2 Sensorik und Beobachtungen}}$

- **RayPerceptionSensor3D-Konfiguration**
  - Insgesamt $\color{pink}\textrm{13}$ spezialisierte `RayPerceptionSensor3D`-Komponenten.
  - Über `SetRayPerceptionTags()` sind vier Tags detektierbar:
    - `"target"` (Zielobjekte)
    - `"killPlayer"` (tödliche Hindernisse)
    - `"ground"` (begehbare Flächen)
    - `"wall"` (Wände)

- **Frontale/seitliche Umgebungserkennung**
  - Fokussierter Frontalbereich:  
    2 Strahlen pro Seite, Öffnungswinkel **21°**, Reichweite **80 Einheiten**, `SphereCastRadius = 0.47` (präzise Fernerkennung in Blickrichtung).
  - Weitwinkel-Frontalbereich:  
    3 Strahlen pro Seite, Öffnungswinkel **79°**, Reichweite **50 Einheiten**, Gesamtabdeckung ca. **155°** nach vorne.

- **Frontale Bodenerkennung**
  - Fokussierter Bodennahbereich:  
    Engwinklige Erfassung des Bodens in ca. **5 Einheiten** Entfernung.
  - Weitwinkel-Bodennahbereich:  
    Weitwinklige Erfassung in ca. **12 Einheiten** Entfernung.
  - Zusätzliche 7 `RayFrontDown`-Sensoren:  
    Vertikal nach vorne-unten, Reichweite **10 Einheiten**, `SphereCastRadius = 0.18`, Erkennung nur von `ground`- und `killPlayer`-Layern.

- **Rückwärtige Sensorik**
  - `RayBack`-Sensor, auf separatem, um 180° rotiertem Child-Objekt.
  - 3 Strahlen pro Seite, nahezu **180°** Abdeckung, Reichweite **50 Einheiten**.
  - Zusammen mit den Frontsensoren ergibt sich eine **360°-Rundumüberwachung**.

- **Vektorbeobachtungen (CollectObservations)**
  - Pro Zeitschritt werden über `CollectObservations(VectorSensor sensor)` erfasst:
    - Normalisierte Zielrichtung (3 Werte)
    - Normierte Entfernung zum Ziel (1 Wert, auf [0, 1] skaliert)
    - Bodenkontakt (`isGrounded`, 1 Wert)
    - X-Geschwindigkeit (skaliert auf [-1, 1])
    - Z-Geschwindigkeit (skaliert auf [-1, 1])
    - Y-Geschwindigkeit
  - Insgesamt $\color{pink}\textrm{8}$ Vektorbeobachtungen pro Frame.
  - Durch **Frame Stacking** über 5 Zeitschritte entsteht ein effektiver Zustandsvektor von  
    $\color{pink}\textrm{5 × 8 = 40}$ Vektorbeobachtungen pro Zeitschritt.

- **Raycast-Beobachtungen und Gesamtumfang**
  - Beobachtungen pro Ray-Sensor:  
    `Beobachtungen = (Anzahl Rays) × (Anzahl Detectable Tags + 2) × (Stacked Raycasts)`
    - `+2`: Hit/Miss-Status und normalisierte Distanz pro Strahl.
  - Aus detaillierter Analyse der Sensoren ergeben sich $\color{pink}\textrm{498}$ Raycast-Beobachtungen.
  - **Gesamtbeobachtungen pro Zeitschritt**:  
    `Gesamt = 40 (Vektor) + 498 (Raycasts) =` $\color{pink}\textrm{538}$ `Beobachtungen.`

### ${\textrm{\color{lightgreen}3.3 Aktionsverarbeitung}}$

- **Hybrider Aktionsraum** (`OnActionReceived`)
  - Kontinuierliche Aktionen:
    - Horizontaler Wert: Drehgeschwindigkeit um die Y-Achse (Rotation)
    - Vertikaler Wert: Bewegung entlang der lokalen Z-Achse (vorwärts/rückwärts)
    - Wertebereich: **[-1, 1]**, Übergabe an `MovementController`.
  - Diskrete Aktionen:
    - Sprung (Boolean, 0/1)
    - Rennen (Boolean, 0/1, Erhöhung der Bewegungsgeschwindigkeit)

- **Heuristische Steuerung (manuelle Eingabe)**
  - `Heuristic(in ActionBuffers actionsOut)` liest Eingaben aus dem Unity-Input-System:
    - `Horizontal`/`Vertical`-Achsen → kontinuierliche Aktionen
    - `Space`-Taste → Sprung
    - `LeftShift` → Rennen
  - Eignet sich für Debugging, Demonstrationen und potenziell für Imitation Learning.

- **Bewegungs- und Sprungmechanik**
  - Rotation:  
    `rotationAmount = horizontalInput * currentSpeed * Time.fixedDeltaTime * rotationSpeed`  
    Umsetzung über `transform.Rotate(0, rotationAmount, 0)`.
  - Translation:  
    Bewegungsvektor aus vertikalem Input und Geh-/Laufgeschwindigkeit, direkt in `Rigidbody.velocity` übernommen.
  - Sprung:  
    Bodenprüfung via Raycast nach unten (Toleranz **0,1 Einheiten**); bei Bodenkontakt und Sprungbefehl wird eine Sprungkraft von **30** auf die vertikale Geschwindigkeit angewendet. Ein Schwerkraftmultiplikator von **10** sorgt für realistischere Fallbewegung.

### ${\textrm{\color{lightgreen}3.4 Belohnungslogik (Rewards) und Verhaltenssteuerung}}$

- **Fortschrittsbasierte Belohnungen**
  - Kontinuierliche Bewertung des Fortschritts Richtung Ziel:  
    `progress = (lastDistance - currentDistance) * 0.5f`  
    Annäherung → positive Belohnung, Entfernung → Bestrafung.
  - Intervallbasierte Distanzprüfung (`CheckDistance10()`):
    - Alle **100 Schritte** wird geprüft, ob der Agent mindestens **1 Einheit** vorangekommen ist.
    - Erfolg: **+2,5** Punkte, unzureichender Fortschritt: **-3,75** Punkte.

- **Checkpoint-Belohnungssystem**
  - Gesamtbelohnung **50 Punkte** pro Level, gleichmäßig auf alle Checkpoints verteilt:  
    `Belohnung pro Checkpoint = 50f / AnzahlCheckpoints`.
  - Beim finalen Ziel:  
    - Feste Zielbelohnung: **+5** Punkte
    - Zeitbonus: bis zu **+20** Punkte  
      `timeBonus = Clamp(1f - (StepCount / MaxStep), 0f, 1f) * 20f`
    - Zusätzlicher Siegesbonus: **+10** Punkte.

- **Bewegungsoptimierung**
  - Vorwärtsbewegungen:  
    `movementReward = 0.02f * Clamp(verticalInput, 0f, 1f)` (positive Verstärkung).
  - Drehbewegungen: konstante Strafe **-0,025** pro Schritt (Reduktion exzessiver Rotation).
  - Sprungkonsumption: **-2,5** Punkte pro Sprung → Sprünge werden nur bei Notwendigkeit eingesetzt.

- **Kollisions- und Fehlervermeidung**
  - Wandkollisionen:  
    `reward = -1f * Clamp(impactForce, 0.5f, 3f)` (stärkere Strafe bei härteren Aufprallen).
  - Kollision mit `killPlayer`-Objekten:  
    Strafe **-20** Punkte + distanzbasierte Strafe  
    `distPenalty = currentDistance / maxLevelDistance`.

- **Episoden-Management und Zeitoptimierung**
  - Erfolgreicher Abschluss: Kombination aus Zielbelohnung, Zeitbonus und Siegesprämie → insgesamt bis zu **35** Zusatzpunkte.
  - Abbruch durch Zeitüberschreitung oder tödliche Kollision: Grundstrafe **-20** Punkte + Distanzstrafe.
  - `MaxStep`-Limit verhindert unendlich lange Episoden und fördert direkte, zielgerichtete Strategien.

## ${\textrm{\color{red}4. Trainingsumgebung und Level-Design }}$

### ${\textrm{\color{lightgreen}4.1 Überblick und progressive Komplexität}}$

- Es wurden $\color{pink}\textrm{13}$ unterschiedliche Parkour-Trainingslevel mit variierenden Schwierigkeitsgraden implementiert.
- Ziel ist, den Agenten schrittweise mit neuen Hindernissen, Mechaniken und Aufgaben zu konfrontieren.
- Zwei zentrale Designprinzipien:
  - **Progressive Komplexität**: Aufbau von einfachen Navigationsaufgaben hin zu komplexen, dynamischen Hindernissen und interaktiven Rätseln.
  - **Variabilität für Generalisierung**: Unterschiedliche Varianten (z. B. Anzahl/Zahl der Ziele, zufällige Zielpositionen, dynamische Umgebungsgeometrie) verhindern Overfitting auf starre Layouts.

### ${\textrm{\color{lightgreen}4.2 Hindernistypen und Umgebungselemente}}$

- **Statische Hindernisse**  
  Wände, Blöcke, Säulen → trainieren grundlegende Pfadplanung und Kollisionsvermeidung.

- **Dynamische Hindernisse**  
  Rotierende oder linear bewegte Blöcke/Säulen → erfordern vorausschauende Planung und Timing.

- **Interaktive Elemente**  
  Sammel-/Zielobjekte mit variabler Position und Anzahl sowie Schalter, die Hindernisse deaktivieren.

- **Dynamische Umgebungsgeometrie**  
  Teile des Bodens verschieben sich bei Episodenstarts; der Agent darf sich nicht auf starre Pfade verlassen.

### ${\textrm{\color{lightgreen}4.3 Struktur und Beispiele der Level (Auszug)}}$

- **Level 1 – Grundlegende Navigation**
  - 80×80-Einheiten großer begehbarer Boden, Randbegrenzung, ein einzelnes Zielobjekt.
  - Keine Hindernisse; Fokus auf Vorwärts-/Rückwärtsbewegung und Drehung.
  - Eine Variante: Zielobjekt erscheint zufällig in Randnähe.

- **Level 2 – Grundlegende Navigation mit Hindernisvermeidung**
  - Aufbau ähnlich Level 1, jedoch mit zwei konzentrischen Lavafeldern um die Startposition.
  - `Lava` ist tödlich; der Agent muss Springen lernen und gezielt einsetzen, um die Lava zu überqueren.
  - Eine Variante: Zielobjekt zufällig in Randnähe, ähnlich Level 1.

- **Weitere Level (3–13)**
  - Steigern Komplexität z. B. durch Interaktionselemente, Sprungtiming, lineare und chaotische Bewegungen, dynamische Hindernisse (Versionen V1–V3) und ein Level mit randomisierten Hindernissen.

- **Generalisierungs-Level**
  - Level, das im Training nicht verwendet wurde.
  - Kombiniert bekannte Umgebungs- und Hinderniselemente in neuer Konfiguration.
  - Dient ausschließlich zur Messung der Generalisierungsfähigkeit.

### ${\textrm{\color{lightgreen}4.4 Levelanzahl und Varianten im finalen Trainingssetup}}$

- Insgesamt $\color{pink}\textrm{13}$ Grundlevel (Level 1–13) + ein dediziertes Generalisierungs-Level.
- In **Trainingsstrategie 6** (variantenbasierter Levelaufbau):
  - Ein Build enthält alle Level und Varianten.
  - Insgesamt $\color{pink}\textrm{31}$ unterschiedliche Parkours (13 Level + Varianten).
  - Manche Level (1, 2, 3, 10–13) ohne Varianten werden mehrfach integriert, sodass die Zahl der Parkours im Build auf $\color{pink}\textrm{78 (13 × 6)}$ pro Build erhöht wird, um eine gleichmäßige Verteilung zu erreichen.

## ${\textrm{\color{red}5. Trainingsstrategien und Hyperparameter }}$

### ${\textrm{\color{lightgreen}5.1 Lernalgorithmus und Netzwerkarchitektur}}$

- **Algorithmus**
  - `trainer_type: ppo` (${\textrm{\color{orange}Proximal Policy Optimization}}$)
  - Geeignet für kontinuierliche bzw. hybride Aktionsräume mit komplexer Bewegungsdynamik.

- **Netzwerkarchitektur**
  - `network_type: lstm` (${\textrm{\color{orange}Long Short-Term Memory}}$)
  - `hidden_units: 512`, `num_layers: 5`
  - `memory: { sequence_length: 64, memory_size: 128 }`
  - `normalize: true` (Beobachtungsnormalisierung)
  - `vis_encode_type: simple`

### ${\textrm{\color{lightgreen}5.2 Zentrale Hyperparameter (Auszug)}}$

- **Optimierung und Stabilität**
  - `batch_size: 2048`
  - `buffer_size: 40960` (20× Batchgröße)
  - `learning_rate: 0.0005` (mit linearem Schedule)
  - `beta: 0.08` (Entropiekoeffizient, mit linearem Schedule)
  - `epsilon: 0.2` (${\textrm{\color{orange}PPO-Clipping}}$, mit linearem Schedule)
  - `lambd: 0.94` (${\textrm{\color{orange}GAE-Lambda}}$)
  - `num_epoch: 4`

- **Belohnungs- und Trainingsparameter**
  - `gamma: 0.99` (hoher Diskontierungsfaktor)
  - `max_steps: 1000000` (pro Trainingslauf)
  - `time_horizon: 512`
  - `summary_freq: 10000`
  - `threaded: false` (Determinismus / Reproduzierbarkeit)

- **Umgebungs- und Engine-Einstellungen**
  - `num_envs: 7` (parallele Umgebungsinstanzen)
  - `base_port: 5005`
  - `no_graphics: true` (Training ohne Rendering)
  - `time_scale: 3` (beschleunigte Simulation)

### ${\textrm{\color{lightgreen}5.3 Trainingsstrategien 1–6 }}$

- **Strategie 1 – Einzelnes Modell pro Level**
  - Für jedes Level wird ein eigenes Modell trainiert (separate Builds).
  - Transfer über `--initialize-from` von Level zu Level.
  - Vorteile: einfache Struktur, gut messbare Fortschritte.
  - Nachteile: starkes Overfitting, kaum Transfer zwischen Levels, ${\textrm{\color{orange}Catastrophic Forgetting}}$, hoher manueller Überwachungsaufwand.

- **Strategie 2 – Inkrementelles Hinzufügen von Levels**
  - Builds mit schrittweiser Hinzunahme weiterer Levels.
  - Transfer jeweils über `--initialize-from` auf das vorherige Modell.
  - Weniger ${\textrm{\color{orange}Catastrophic Forgetting}}$ als Strategie 1, aber weiterhin Overfitting auf frühe Levels und hoher Build-/Trainingsaufwand.

- **Strategie 3 – Gruppierung ähnlicher Level**
  - Gruppen wie: (1–3), (4–6), (7–8) in separaten Builds.
  - Positiver Transfer innerhalb thematisch ähnlicher Level.
  - Nachteile: ${\textrm{\color{orange}Catastrophic Forgetting}}$, Overfitting und subjektive Gruppeneinteilung.

- **Strategie 4 – Einfache und mittlere Varianten**
  - Für geeignete Levels werden zusätzliche einfache und mittlere Varianten erstellt (durch Entfernen von Checkpoints).
  - Strategien 1–3 werden auf diese Varianten übertragen.
  - Deutlich bessere Lernkurven und robustere Skill-Entwicklung, aber verdreifachte Trainingsanzahl, hoher Konfigurationsaufwand, weiterhin Probleme wie Overfitting und ${\textrm{\color{orange}Catastrophic Forgetting}}$.

- **Strategie 5 – Gemeinsames Training aller Levelvarianten**
  - Ein Build mit allen einfachen, mittleren und schwierigen Varianten.
  - Training mit **einem** Modell über alle Varianten.
  - Vorteile: hohe Datenvielfalt, robuste Performance, kaum ${\textrm{\color{orange}Catastrophic Forgetting}}$, geringe Overfitting-Tendenz.
  - Nachteile: aufwendige Konfiguration, schwierigeres Debugging, potenzielle Überforderung des Agenten, kompliziertere Hyperparameterabstimmung.

- **Strategie 6 – Variantenbasierter Levelaufbau (finales Setup)**
  - Verzicht auf einfache/mittlere Varianten; stattdessen dynamische Varianten.
  - Ein Build mit $\color{pink}\textrm{31}$ unterschiedlichen Parkours (insgesamt $\color{pink}\textrm{78}$ Szeneninstanzen).
  - Training mit **einem** Modell ohne Neuinitialisierung.
  - Vorteile: beste Performance und Robustheit, kaum Overfitting, kein ${\textrm{\color{orange}Catastrophic Forgetting}}$, Agent lernt allgemeine statt level-spezifische Fähigkeiten, keine Interaktion während des Trainings nötig.
  - Nachteile: ähnlich Strategie 5 (Debugging, Hyperparameter-Suche, anfängliche Überforderung).

### ${\textrm{\color{lightgreen}5.4 Eigene Automatisierungslösung }}$

- **Motivation:** Beim Training zeigte sich: Mehrere kurze Trainingsiterationen (z. B. 20×1 Mio. Schritte) liefern bessere Ergebnisse als ein langer Lauf.
- **Prolem:** Der PPO-Algorithmus reduziert `learning_rate`, `beta` und `epsilon` automatisch. Durch häufige Neustarts in segmentierten Trainingsläufen sanken diese Werte zu schnell, was das spätere Lernen negativ beeinflusste. → Manuelle Nachjustierung nötig.

- **Lösung: Python-Automatisierungsskript**
  - Automatisiert den gesamten Trainingsprozess, inklusive dynamischer Anpassung von `learning_rate`, `beta` und `epsilon` abhängig von der Gesamtzahl absolvierte Schritte:
    - Frühe Phase (< 3 Mio. Schritte): stärkere Anpassungen
    - Mittlere Phase (3–15 Mio. Schritte): moderat
    - Späte Phase (> 15 Mio. Schritte): minimale oder keine Anpassungen
  - Automatische Verwaltung und Anpassung der YAML-Konfigurationsdateien.
  - Nahtlose Fortsetzung von Trainingsphasen auf Basis vorheriger Modelle.
  - Umfassendes Logging (Parameteränderungen mit Zeitstempel).

- **Manuelle Konfiguration vor Skriptstart**
  - Steuerungsparameter: `first_run`, `continue_training`, `based_on`
  - Pfade: `WORK_DIR`, `ENV_PATH`, `RUN_ID`, `BASED_ON_ID`, `LOG_DIR`, `CONFIG_FILE` etc.
  - Trainingsparameter: `MAX_TOTAL_STEPS`, `STEP_INCREMENT`, `LR_INCREMENT` sowie konkrete Anpassungsregeln für `learning_rate`, `beta`, `epsilon`.

- **Vorteile der Automatisierung**
  - Keine manuellen Eingriffe während langer Traininsläufe
  - Einheitliche, nachvollziehbare Hyperparameterentwicklung
  - Weniger Fehler durch manuelle Bedienung
  - Ermöglicht unbeaufsichtigtes Langzeittraining
  - Vollständige Dokumentation aller Änderungen

### ${\textrm{\color{lightgreen}5.5 Trainingsablauf und TensorBoard}}$

- **Start des Trainings (Standardverfahren)**

```bash
mlagents-learn <pfad_zur_config.yaml> --run-id=<modell_name> --env="<pfad_zum_build>"
```

- **Start von TensorBoard**

```bash
tensorboard --logdir results
```

- **Wichtige Metriken in TensorBoard**
  - Cumulative Reward (kumulierte Belohnung)
  - Policy Loss und Value Loss
  - Lernrate, Beta und Epsilon-Verläufe
  - Win/Death-Statistiken, level-spezifische Erfolgsquoten, zeitbezogene Metriken

## ${\textrm{\color{red}6. Ergebnisse und Evaluation}}$

### ${\textrm{\color{lightgreen}6.1 Leistungsanalyse des finalen Modells (Strategie 6)}}$

- **Level-spezifische Erfolgsquoten**
  - Grundlagen-Level (1–3): nahezu perfekte Erfolgsquoten; Basisfähigkeiten zuverlässig erlernt.
  - Präzisions-Parkours (4–6): dauerhaft hohe Erfolgsraten; Agent nutzt z. B. in Level 6 bei deaktivierten Checkpoints gezielt Abkürzungen.
  - Manövrier-Level (7–9): Agent zeigt präzises Landen auf Plattformrändern (Level 7) und flüssige Kurvenbewegungen im Zickzack-Level 9.
  - Dynamische Hindernis-Level (10–13): höhere Fehleranfälligkeit, aber weiterhin gute Gesamtleistung, auch bei komplexen Bewegungsmustern.

- **Emergente Verhaltensweisen**
  - Vorausschauendes Verhalten: Agent wartet in Engstellen auf günstige Konstellationen dynamischer Hindernisse.
  - Risikoanpassung: In stark zufälligen Levels (z. B. Level 13) bevorzugt der Agent kurze, vorsichtige Bewegungen.
  - Kostenbewusstes Aktionsverhalten: Aufgrund negativer Sprungbelohnungen nutzt der Agent Sprünge nur bei Notwendigkeit (z. B. Level 2, 3, 4, 7, 8).
  - Strategieanpassung: Direkte Zielverfolgung in einfachen Levels, abwartendes risikoorientiertes Verhalten in komplexen Szenarien.

### ${\textrm{\color{lightgreen}6.2 Generalisierungsfähigkeit}}$

- **Testsetup**
  - Spezielles Generalisierungs-Level, das im Training nicht vorkommt.
  - Neue Kombination bekannter Elemente, deutlich unterschiedlicher Levelaufbau.
  - Bewertung ausschließlich über **Win/Death Ratio**, da Belohnungen und Episodendauer levelabhängig sind.

- **Ergebnisse**
  - Generalisierungs-Level: Überlebensrate  $\color{pink}\textrm{43–53 \%}$.
  - Trainings-Level: durchschnittliche Überlebensrate ca. $\color{pink}\textrm{92 \%}$.
  - Differenz von ca. $\color{pink}\textrm{42–49 Prozentpunkten}$ spiegelt erhöhten Schwierigkeitsgrad und strukturelle Unterschiede wider, zeigt aber dennoch adaptive Fähigkeiten.

- **Interpretation**
  - Positiv: Agent verfügt über Navigations- und Vermeidungsstrategien, die auf unbekannte Kontexte übertragbar sind; Varianten- und Curriculum-basiertes Training (insb. Strategie 6) stärkt abstrahierbare Fähigkeiten.
  - Grenzen: Neuartige Anordnungen überfordern das Modell teilweise; Abhängigkeit von Trainingsmustern und Limitierungen der LSTM-Architektur in hochvariablen Szenarien.
  - Im Vergleich zu Strategien 1–5 wären dort aufgrund dokumentierter Probleme (Overfitting, ${\textrm{\color{orange}Catastrophic Forgetting}}$, geringere Erfolgsraten) deutlich schwächere Generalisierungsergebnisse zu erwarten.
## ${\textrm{\color{red}7. Fazit und Ausblick }}$

### ${\textrm{\color{lightgreen}7.1 Zusammenfassung der Ergebnisse}}$

- Erfolgreiche Entwicklung, das Training und die Evaluation eines ML-Agenten auf Basis des Unity ML-Agents Toolkits.
- Eigens entwickelte Parkour-Level fördern gezielt Navigation, präzises Springen, situationsabhängiges Handeln und Interaktion mit Umgebungselementen.
- Kombination von ${\textrm{\color{orange}Domain Randomization}}$, ${\textrm{\color{orange}Curriculum Learning}}$, fein abgestimmter Reward-Struktur, umfangreicher Sensorik (Raycasts + Vektorbeobachtungen) und ${\textrm{\color{orange}LSTM}}$-basiertem ${\textrm{\color{orange}PPO}}$ führt zu stabilen Lernkurven und hohen Erfolgsraten.
- **Trainingsstrategie 6** erweist sich als leistungsfähigste Strategie mit hoher Robustheit und nachgewiesener Generalisierungsfähigkeit auf ein neues Testlevel.

### ${\textrm{\color{lightgreen}7.2 Bewertung des Lösungsansatzes und Limitationen}}$

- **Stärken**
  - Modularer Aufbau der Level und Komponenten; strikte Trennung von Szenen-, Level- und Komponentenlogik.
  - Flexible Umschaltung von Trainingsmodi und Einsatz eines Automatisierungsskripts.
  - Kombination aus progressivem Schwierigkeitsgrad, strukturierter Belohnungslogik und vielseitigen Szenarien ermöglicht sowohl reaktive als auch planungsbasierte Strategien.
  - PPO mit LSTM liefert stabile, nachvollziehbare Lernkurven.

- **Wesentliche Limitationen**
  - Fokus auf ein spezifisches, klar strukturiertes Level-Framework; eingeschränkte Vielfalt der Umgebungen.
  - Verzicht auf visuelle Beobachtungen (ausschließlich numerische Daten wie Position, Geschwindigkeit, Raycasts).
  - Hohe Sensitivität gegenüber Hyperparametern und Reward-Design; komplexe Abstimmung erforderlich.
  - Erklärbarkeit: Interne Entscheidungsprozesse des neuronalen Netzes bleiben schwer interpretierbar.
  - Evaluationsrahmen beschränkt auf die Unity-Umgebung; keine externen Benchmarks oder andere Frameworks einbezogen.

### ${\textrm{\color{lightgreen}7.3 Ausblick (zukünftige Erweiterungen)}}$

- **Curriculum Learning & adaptive Schwierigkeitsanpassung**
  - Einsatz von Ansätzen wie „Reverse Curriculum Learning“ oder „Automatic Level Progression“, um Levelschwierigkeit automatisch an das Kompetenzniveau des Agenten anzupassen.

- **Self-Play & Multi-Agenten-Systeme**
  - Einführung kooperativer oder kompetitiver Agenten, die voneinander lernen oder sich gegenseitig herausfordern; Potenzial für emergente Strategien wie Teamarbeit oder taktisches Verhalten.

- **Verbesserte Sensorik & Wahrnehmung**
  - Ergänzung der Raycast-Sensorik durch visuelle Beobachtungen (Kamerabilder) oder LIDAR-ähnliche Scans, um visuell kodierte Informationen (z. B. Symbole auf Schaltern) zu nutzen.

- **Explainable AI (XAI)**
  - Integration von XAI-Ansätzen zur besseren Interpretation der Entscheidungen des Agenten und zur Identifikation weiterer Optimierungsmöglichkeiten.

