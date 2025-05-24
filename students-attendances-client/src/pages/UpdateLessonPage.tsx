import PageCard from "@components/pagecard/PageCard";
import type { UpdateLessonRequest } from "@core/models/lesson-models";
import { useLessonStore } from "@core/store/store";
import { lessonService } from "@services/LessonService";
import { AxiosError } from "axios";
import { LucideCircleArrowLeft, Trash } from "lucide-react";
import { useEffect, useState, type JSX } from "react";
import { Button, Col, Form, Row, Stack } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";

export default function UpdateLessonPage(): JSX.Element {
	
	const { fetchLessons, course, lessons } = useLessonStore()

	const [ name, setName ] = useState<string>('')
	const [ lessonData, setLessonData ] = useState({
		date: undefined as string | undefined,
		startTime: undefined as string | undefined,
		endTime: undefined as string | undefined
	});
	const handleDateTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setLessonData(prev => ({ ...prev, [name]: value }));
  };

	const navigate = useNavigate()
	const { id } = useParams()

	const handleLessonDelete = async () => {
		try {
			const response = (await lessonService.deleteLesson(parseInt(id!))).data
			console.log(response)
			navigate(`/processing/${response}/-2`)
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}
	const handleLessonUpdate = async () => {
		if (!lessonData.date || !lessonData.startTime || !lessonData.endTime) {
			return alert('Время проведения имеет некорректное значение')
		}
		if (!isStartTimeBeforeOrEqualEndTime(lessonData.startTime, lessonData.endTime)) {
			return alert('Время окончания занятия меньше, чем время начала')
		}
		if (name.length <= 3) return alert('Название слишком короткое')
		const request: UpdateLessonRequest = {
			startTime: toString(lessonData.date, lessonData.startTime),
			endTime: toString(lessonData.date, lessonData.endTime),
			description: name,
			lessonId: parseInt(id!)
		}
		console.log(request)
		try {
			const response = (await lessonService.updateLesson(request)).data
			console.log(response)
			navigate(`/processing/${response}/-2`)
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}
	
	useEffect(() => {
		if (!id || !course) {
			navigate('/dashboard')
			return
		}
		(async () => {
			await fetchLessons(course.externalId)
			const current = lessons.find(it => it.externalId == parseInt(id))
			if (current) {
				setName(current.description)
				setLessonData(transformTimeRange(current.startTime, current.endTime))
			}
		})().catch(error => console.log(error))
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [course])

	if (!course) return <></>
  return (
		<PageCard>
			<Stack gap={3} className="w-100 p-2 p-md-4 pb-md-5 shadow rounded bg-white bg-opacity-75">
				<div className='d-flex flex-row mb-4 justify-content-start align-items-center gap-4'>
					<div className='gradient-input-wrapper p-1' onClick={() => navigate(-1)} style={{
								cursor: 'pointer'
							}}>
						<LucideCircleArrowLeft color='white' size={26}/>
					</div>
					<h3 className="m-0 fs-3" style={{textTransform: 'uppercase'}}>Обновление урока</h3>
				</div>
				<div>
					<h5 className="mb-3">Основная информация</h5>
					<Row className="justify-content-center">
						<Col sm='12' md='8' lg='6'>
							<Form.Group>
								<Form.Label className="w-100 mx-2 mb-0 text-start">Название урока:</Form.Label>
								<div className="gradient-input-wrapper mb-3">
									<Form.Control
										type="text"
										className="gradient-input"
										value={(() => {
											const filter = name.replace(/<\/?[^>]+(>|$)/g, '')
											return filter == '' ? 'Обычное занятие' : filter
										})()}
										onChange={(e) => setName(e.target.value)}
										placeholder="Введите название урока"
									/>
								</div>
							</Form.Group>
						</Col>
					</Row>
				</div>
				<div className="mb-4">
					<h6>Дата и время проведения занятия</h6>
					<Row className="justify-content-center">
						<Col xs='12' sm='12' md='6' lg='4'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0 text-start">Дата занятия:</Form.Label>
								<Form.Control type="date" name="date" required
										value={lessonData.date}
              			onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
						<Col xs='6' sm='6' md='3' lg='2'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0">Начало</Form.Label>
								<Form.Control type="time" name="startTime" required 
									value={lessonData.startTime}
									onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
						<Col xs='6' sm='6' md='3' lg='2'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0">Конец</Form.Label>
								<Form.Control type="time" name="endTime" required
									value={lessonData.endTime}
									onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
					</Row>
				</div>
				<div className='d-flex flex-row justify-content-center gap-4'>
					<div className='d-flex justify-content-center'>
						<div className="gradient-input-wrapper mb-3">
							<Button className='border-0 bg-white' 
								style={{
									borderRadius: '10px',
									color: 'black'
								}}
								onClick={() => handleLessonDelete()}
							>
								<Trash size={20} color='crimson'/>
							</Button>
						</div>
					</div>
					<div className='d-flex justify-content-center'>
						<div className="gradient-input-wrapper mb-3">
							<Button className='border-0 bg-transparent' style={{width: '200px'}}
								onClick={() => handleLessonUpdate()}
							>
								Обновить урок
							</Button>
						</div>
					</div>
				</div>
			</Stack>
		</PageCard>
	)
}
function transformTimeRange(startTime: string, endTime: string) {
  const start = new Date(startTime);
  const end = new Date(endTime);
  const pad = (n: number) => n.toString().padStart(2, '0');
  const date = start.toISOString().split('T')[0];
  const formatTime = (d: Date) => `${pad(d.getHours())}:${pad(d.getMinutes())}`;
  return {
    date,
    startTime: formatTime(start),
    endTime: formatTime(end),
  };
}

function isStartTimeBeforeOrEqualEndTime(startTime: string, endTime: string): boolean {
  const toMinutes = (time: string): number => {
    const [hours, minutes] = time.split(':').map(Number)
    return hours * 60 + minutes
  }
  return toMinutes(startTime) <= toMinutes(endTime)
}

const toString = (date: string, time: string): string => `${date}T${time}:00Z`