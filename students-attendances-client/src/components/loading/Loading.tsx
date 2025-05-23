import type { JSX } from "react";
import './Loading.css'

interface Props {
	children: React.ReactNode
	isLoading: boolean
}

export default function Loading({ children, isLoading }: Props): JSX.Element {
	return isLoading 
		? <div className="d-flex flex-column justify-context-center align-items-center gap-2">
				<SvgGradientSpinner size={40}/> 
				<span>Загрузка...</span>
			</div>
		: <>{children}</>
}

const SvgGradientSpinner: React.FC<{ size?: number }> = ({ size = 64 }) => (
  <svg
    width={size}
    height={size}
    viewBox="0 0 50 50"
    xmlns="http://www.w3.org/2000/svg"
    role="status"
    style={{ animation: 'spin .8s linear infinite' }}
  >
    <defs>
      <linearGradient id="spinnerGradient" x1="0%" y1="0%" x2="100%" y2="100%">
        <stop offset="0%" stopColor="#ff6f61" />
        <stop offset="100%" stopColor="#6a11cb" />
      </linearGradient>
    </defs>
    <circle
      cx="25"
      cy="25"
      r="20"
      stroke="url(#spinnerGradient)"
      strokeWidth="5"
      fill="none"
      strokeLinecap="round"
      strokeDasharray="90 150"
      strokeDashoffset="0"
    />
  </svg>
);