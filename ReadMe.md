# ${\textrm{\color{green}Bachelorarbeit-Demo}}$

Dieses Repository enthält eine ${\textrm{\color{orange}aufbereitete Demo-Version}}$ des Unity-Projekts, das meine Bachelorarbeit zum Thema $\textrm{\color{orange}Reinforcement Learning}$ in Unity begleitet. Es dient als kompakte, exemplarische Darstellung der wichtigsten Bestandteile.

⚠️Hinweis: Im Vergleich zur Originalversion der Bachelorarbeit wurden größere Teile des ursprünglichen Codes und einige Komponenten entfernt oder vereinfacht. Die Demo konzentriert sich auf die wesentlichen Mechaniken des trainierten ML-Agenten und wurde um ein Menü und UI erweitert.

Das Readme selbst stellt eine kurze, kompakte Zusammenfassung der schriflichten Bachelorarbeit dar.

## ${\textrm{\color{red}1. Projektübersicht und Struktur }}$

- **[Bachelorarbeit.md](Bachelorarbeit.md)**  
  Kompakte, textuelle Zusammenfassung der schriflichten Bachelorarbeit.

- **[Dokumente](Dokumente/)**  
  Enthält ergänzende Dokumente wie Installations- und Trainingsanleitungen sowie weitere Materialien zur Bachelorarbeit.
  - [How_To_Train.md](Dokumente/How_To_Train.md)
  - [How_To_Install.md](Dokumente/Install/How_To_Install.md)

- **[Assets](Assets/)**  
  Unity-Assets, Szenen, Skripte und Prefabs des Demo-Projekts.

- **[config](config/)**  
  Konfigurationsdateien für das Training mit dem Unity ${\textrm{\color{orange}ML-Agents Toolkit}}$ (z. B. YAML-Configs).

- **[results](results/)**  
  Trainingsergebnisse, Log-Dateien und TensorBoard-Daten der verschiedenen Trainingsläufe.


## ${\textrm{\color{red}2. Demo-Build starten (ohne Unity)}}$

1. **Passenden Build auf der GitHub-Release-Seite herunterladen**  
   - Wechsle im Repository auf den Reiter **"Releases"**.  
   - Wähle den Build für dein Betriebssystem: **Windows**, **Linux** oder **macOS**.

2. **Archiv entpacken**  
   - Das heruntergeladene ZIP/Archiv in einen beliebigen Ordner entpacken.

3. **Demo starten**  
   - Die passende ausführbare Datei für dein System starten (z. B. `.exe` unter Windows).  
   - Es öffnet sich das Demo-Spiel mit dem trainierten Agenten.


## ${\textrm{\color{red}3. Steuerung der Demo }}$

### ${\textrm{\color{lightgreen}3.1 Allgemeine Steuerung}}$

- **Pause Menü öffnen**: `Tab`

### ${\textrm{\color{lightgreen}3.2 Steuerung im "Watch AI"-Modus}}$

- Der trainierte ${\textrm{\color{orange}ML-Agent}}$ steuert die Spielfigur vollständig autonom.
- Du beobachtest, wie der Agent die Parkour-Level absolviert, Hindernissen ausweicht und Checkpoints bzw. Ziele erreicht.
- Dieser Modus dient vor allem zur **Demonstration des erlernten Verhaltens** und zur qualitativen Bewertung der Agentenleistung.

- **Kamera bewegen**: `W`, `A`, `S`, `D`
- **Hoch**: `Space`
- **Runter**: `Left Control`
- **Speed**: `Left Shift`

### ${\textrm{\color{lightgreen}3.3 Steuerung im "Play by yourself"-Modus}}$

- Die Spielfigur wird vollständig manuell über Tastatureingaben gesteuert.
- Du kannst dieselben Level wie der Agent spielen und dein eigenes Verhalten mit dem des Agenten vergleichen.
- Eignet sich, um ein Gefühl für den **Schwierigkeitsgrad** der Level und für die vom Agenten erlernten Strategien zu bekommen.

- **Bewegen**: `W`, `A`, `S`, `D` 
- **Springen**: `Space`
- **Rennen**: `Left Shift`

