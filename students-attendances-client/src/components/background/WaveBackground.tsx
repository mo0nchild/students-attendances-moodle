import { useEffect, useState, type JSX } from 'react';
import './WaveBackground.css';

export default function WaveBackground(): JSX.Element {

  const [wavePaths, setWavePaths] = useState<string[]>([]);

  useEffect(() => {
    const updateWavePaths = () => {
      const styles = getComputedStyle(document.documentElement);
      const offsetBase = parseInt(styles.getPropertyValue('--wave-offset')) || 32;
      const baseY = parseInt(styles.getPropertyValue('--wave-base')) || 192;

      const newPaths = Array.from({ length: 4 }, (_, i) => {
        const offset = i * offsetBase;
        return `M0,${baseY - offset} C360,${baseY + 128 - offset} 1080,${baseY - 128 + offset} 1440,${baseY - offset} L1440,320 L0,320 Z`;
      });

      setWavePaths(newPaths);
    };

    updateWavePaths(); // initial
    window.addEventListener('resize', updateWavePaths);
    return () => window.removeEventListener('resize', updateWavePaths);
  }, []);

  return (
    <div className="waves-wrapper">
      <div className="gradient-layer" />
      <div className="waves-mask">
        {wavePaths.map((path, i) => (
          <svg key={i} className={`wave wave${i + 1}`} viewBox="0 0 1440 320" preserveAspectRatio="none">          
            <path d={path} fill="white" opacity={`${0.15 + i * 0.15}`}>
              <animate
                attributeName="d"
                dur={`${12 + i * 3}s`}
                begin={`${i * 2}s`}
                repeatCount="indefinite"
                values={getAnimatedValues(path)}
              />
            </path>
          </svg>
        ))}
      </div>
    </div>
  );
}

function getAnimatedValues(basePath: string): string {
  return `${basePath};
    ${shiftPath(basePath, -32)};
    ${basePath}`;
}

function shiftPath(path: string, deltaY: number): string {
  return path.replace(/,(\d+)/g, (_, y) => {
    const newY = Math.max(0, parseInt(y) + deltaY);
    return `,${newY}`;
  });
}
