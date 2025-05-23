import Loading from "@components/loading/Loading";
import PageCard from "@components/pagecard/PageCard";
import type { LessonSyncItem } from "@core/models/lesson-models";
import { lessonService } from "@services/LessonService";
import { AxiosError } from "axios";
import { useEffect, useState, type JSX } from "react";
import { Button } from "react-bootstrap";
import { useLocation, useNavigate, useParams } from "react-router-dom";

export default function ProcessingWaitPage(): JSX.Element {
	const [ loading, setLoading ] = useState<boolean>(true)
	const [ error, setError ] = useState<string>()

	const { uuid, back } = useParams()
	const navigate = useNavigate()
	const location = useLocation();
	const [ state, setState ] = useState<LessonSyncItem>()

	const checkProcessing = async () => {
		try {
			const response = (await lessonService.checkProcessing(uuid!)).data
			if (response.status == 'Processing') {
				setTimeout(() => checkProcessing(), 500)
			}
			else {
				setLoading(false)
				if (response.status == 'Failed') setError('Не удалось')
				else setState(response)
			}
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			navigate(-1)
			console.log(error)
		}
	}

	useEffect(() => {
		console.log(location.state)
		if (uuid) checkProcessing()
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [])

	useEffect(() => {
		if (!uuid) navigate('/dashboard')
	}, [navigate, uuid])
  return (
		<PageCard>
			<div className="d-flex justify-content-center">
				<div className='p-2 w-75 w-md-50 py-4 p-md-5 shadow rounded bg-white bg-opacity-75'>
					<Loading isLoading={loading}>
						<div className='d-flex flex-column align-items-center gap-3 mt-4'>
							{ error && error.length > 0 
								? <h5 style={{color: 'crimson'}}>Произошла ошибка</h5>
								: <h5 style={{color: 'green'}}>Успешно выполнено</h5>
							}
							<div className="gradient-input-wrapper mb-3">
								<Button className='border-0 bg-transparent' onClick={() => {
									if (location.state && typeof (location.state.prev) === 'string') {
										if (location.state.prev.includes('/lessons/attendance/') && state) {
											navigate(`/lessons/attendance/${state.externalId}`)
										}
									}
									else navigate(back ? parseInt(back) : -1)
								}}>
									Продолжить
								</Button>
							</div>
						</div>
					</Loading>
				</div>
			</div>
		</PageCard>
	)
}